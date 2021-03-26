using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyalAI : SumoBaseAI
{
    [Header("References")]
    [SerializeField] GameHandler GH;
    [SerializeField] Transform objectiveCircle;

    [Header("Variables")]
    [SerializeField] float distanceForAttack;
    [SerializeField] float distanceForDefence;
    [SerializeField] float enemyTargetingRotationOffset;
    [SerializeField] float pickupTargetingRotationOffset;
    [SerializeField] float pickupDistance;
    List<Sumo> otherSumos;
    Vector2 objectiveCenter;

    protected override void Start()
    {
        base.Start();
        otherSumos = new List<Sumo>(FindObjectsOfType<Sumo>());
        otherSumos.Remove(this);
        objectiveCenter = new Vector2(objectiveCircle.position.x, objectiveCircle.position.z);
        destination = objectiveCenter;
    }

    // Update is called once per frame
    void Update()
    {
        // Do I have the most points? -> be defensive
        bool isDefensive = GetComponent<Points>().points > GetSumoWithMostPoints(otherSumos).GetComponent<Points>().points;

        if (AvoidAttacks())
            return;

        if (EngageWithPowerup())
            return;

        // Attack() doesn't happen in case we're defensive
        if (isDefensive || !Attack())
        {
            destination = objectiveCenter;
            currObjective = AiObjective.idle;
        }
    }

    bool AvoidAttacks()
    {
        if (!isDodging) // If dodge is not on cooldown
        {
            List<Sumo> SumosWithinRange = GetSumoWithinRange(otherSumos, transform.position, distanceForDefence);
            Sumo targetSumo = GetClosestPushingSumo(SumosWithinRange);

            if (targetSumo == null)
            {
                return false;
            }

            Quaternion targetRotToMe = Quaternion.LookRotation(transform.position - targetSumo.transform.position);
            Quaternion targetCurrRot = targetSumo.transform.localRotation;

            // Is rotated my way (or close enough - within the angle offset) -> might hit me
            if (Quaternion.Angle(targetCurrRot, targetRotToMe) <= enemyTargetingRotationOffset)
            {
                currObjective = AiObjective.dodge;
                return true;
            }
        }

        return false;
    }

    bool EngageWithPowerup()
    {
        // Currently only targeting point bundles because other powerups feel useless
        List<PowerUpPointBundle> powerups = new List<PowerUpPointBundle>(FindObjectsOfType<PowerUpPointBundle>());
        PowerUpPointBundle closestPowerupToMe = GetClosestGameObject(powerups.ConvertAll(p => p.gameObject), transform.position)?.GetComponent<PowerUpPointBundle>();
        if (closestPowerupToMe != null)
        {
            float myDistanceToClosestPowerup = Vector3.Distance(transform.position, closestPowerupToMe.transform.position);

            if (myDistanceToClosestPowerup <= pickupDistance)
            {
                Quaternion rotTowardsTarget = Quaternion.LookRotation(closestPowerupToMe.transform.position - transform.position);
                Quaternion myCurrRotation = transform.localRotation;

                // Push towards the powerup
                if (!isPushing && Quaternion.Angle(myCurrRotation, rotTowardsTarget) <= pickupTargetingRotationOffset)
                {
                    currObjective = AiObjective.push;
                }

                destination = new Vector2(closestPowerupToMe.transform.position.x, closestPowerupToMe.transform.position.z);
                rotateToY = rotTowardsTarget.eulerAngles.y;

                return true;
            }
        }

        return false;
    }

    bool Attack ()
    {
        List<Sumo> SumosWithinRange = GetSumoWithinRange(otherSumos, transform.position, distanceForAttack);
        Sumo targetSumo = GetSumoWithMostPoints(SumosWithinRange);
        Quaternion rotTowardsTarget = Quaternion.LookRotation(targetSumo.transform.position - transform.position);
        Quaternion myCurrRotation = transform.localRotation;

        // Checking if distance between center of circle to sumo is less or equals than the radius -> is inside the circle
        bool isSumoInCircle = Vector3.Distance(targetSumo.transform.position, objectiveCenter) <= (GH.ringCollider.radius * GH.ringCollider.transform.localScale.y);

        if (isSumoInCircle)
        {
            if (!isPushing && Quaternion.Angle(myCurrRotation, rotTowardsTarget) <= enemyTargetingRotationOffset)
            {
                currObjective = AiObjective.push;
            }

            float angleTowardsClosestSumo = rotTowardsTarget.eulerAngles.y;
            destination = new Vector2(targetSumo.transform.position.x, targetSumo.transform.position.z);
            rotateToY = angleTowardsClosestSumo;

            return true;
        }

        return false;
    }

    Sumo GetSumoWithMostPoints(List<Sumo> sumos)
    {
        sumos.Sort((a, b) =>
        {
            float aPoints = a.GetComponent<Points>().points;
            float bPoints = b.GetComponent<Points>().points;
            return aPoints.CompareTo(bPoints);
        });

        if (sumos.Count > 0)
            return sumos[sumos.Count - 1];
        return null;
    }

    List<Sumo> GetSumoWithinRange(List<Sumo> sumos, Vector3 point, float range)
    {
        sumos.FindAll(sumo =>
        {
            float distanceTo = Vector3.Distance(point, sumo.transform.position);
            return distanceTo <= range;
        });

        return sumos;
    }

    Sumo GetClosestPushingSumo(List<Sumo> sumos)
    {
        return GetClosestGameObject(sumos.FindAll(s => s.isPushing).ConvertAll(s => s.gameObject), transform.position)?.GetComponent<Sumo>();
    }

    GameObject GetClosestGameObject(List<GameObject> objects, Vector3 pos)
    {
        objects.Sort((a, b) =>
        {
            float distanceToA = Vector3.Distance(pos, a.transform.position);
            float distanceToB = Vector3.Distance(pos, b.transform.position);
            return distanceToA.CompareTo(distanceToB);
        });

        if (objects.Count > 0)
            return objects[0];
        return null;
    }
}