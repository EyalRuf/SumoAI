using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyalAI : SumoBaseAI
{
    [Header("References")]
    [SerializeField] Transform objectiveCircle;

    // Objectives
    List<Sumo> otherSumos;
    Vector2 objectiveCenter;
    float objectiveRadius = 10;
    [SerializeField] float minSumoRangeForGeneralEngagement;

    // Attack
    [SerializeField] float distanceForAttack;

    // Defence
    [SerializeField] float distanceForDefence;
    [SerializeField] float opponentSumoTargetingOffset;

    // ### Ideas ###

    // Don't attack people who have less points than u

    // Ring center + dodging/pushing
    // Sumos that are close and pushing -> dodge / push to their direction

    // Sumo Projections?
    // Obstacles?
    // Powerups?
    
    // Target sumo with most points

    protected override void Start()
    {
        base.Start();
        otherSumos = new List<Sumo>(FindObjectsOfType<Sumo>());
        otherSumos.Remove(this);
        objectiveCenter = new Vector2(objectiveCircle.position.x, objectiveCircle.position.z);
        objectiveRadius = objectiveCircle.GetComponent<SphereCollider>().radius * objectiveCircle.transform.localScale.y;
        destination = objectiveCenter;
    }

    // Update is called once per frame
    void Update()
    {
        // Do I have the most points? -> be defensive
        bool defenceFlag = GetComponent<Points>().points > GetSumoWithMostPoints(otherSumos).GetComponent<Points>().points;
        
        //// Closest to powerup? GO!
        //List<Powerup> powerups = new List<Powerup>(FindObjectsOfType<Powerup>());
        //Powerup closestPowerupToMe = GetClosestGameObject(powerups.ConvertAll(p => p.gameObject), transform.position).GetComponent<Powerup>();
        //float myDistanceToClosestPowerup = Vector3.Distance(transform.position, closestPowerupToMe.transform.position);
        //List<Sumo> sumosCloserToPowerupThanMe = GetSumoWithinRange(otherSumos, closestPowerupToMe.transform.position, myDistanceToClosestPowerup);

        //if (sumosCloserToPowerupThanMe.Count == 0)
        //{

        //} else
        //{

        //}


        List<Sumo> SumosWithinRange = GetSumoWithinRange(otherSumos, transform.position, minSumoRangeForGeneralEngagement);
        Sumo closestSumo = GetClosestGameObject(SumosWithinRange.ConvertAll(s => s.gameObject), transform.position).GetComponent<Sumo>();
        Sumo closestPushingSumo = GetClosestPushingSumo(SumosWithinRange);
        Sumo targetSumo = closestSumo;

        if (closestPushingSumo != null && Vector3.Distance(closestPushingSumo.transform.position, transform.position) <= distanceForDefence)
        {
            targetSumo = closestPushingSumo;
        }

        Quaternion rotTowardsTarget = Quaternion.LookRotation(targetSumo.transform.position - transform.position);
        Quaternion targetRotToMe = Quaternion.LookRotation(transform.position - targetSumo.transform.position);
        Quaternion myCurrRotation = transform.localRotation;
        Quaternion targetCurrRot = targetSumo.transform.localRotation;

        // Used push and is rotated my way (or close enough - within the angle offset) -> might hit me
        if (targetSumo.isPushing && Quaternion.Angle(targetCurrRot, targetRotToMe) <= opponentSumoTargetingOffset)
        {
            if (!isDodging)
            {
                currObjective = AiObjective.dodge;
            } 
            else if (!isPushing && Quaternion.Angle(myCurrRotation, rotTowardsTarget) <= opponentSumoTargetingOffset)
            {
                currObjective = AiObjective.push;
            }
        }

        if (!defenceFlag)
        {
            SumosWithinRange = GetSumoWithinRange(otherSumos, transform.position, distanceForAttack);
            targetSumo = GetSumoWithMostPoints(SumosWithinRange);
            rotTowardsTarget = Quaternion.LookRotation(targetSumo.transform.position - transform.position);
            myCurrRotation = transform.localRotation;

            if (!isPushing && Quaternion.Angle(myCurrRotation, rotTowardsTarget) <= opponentSumoTargetingOffset)
            {
                currObjective = AiObjective.push;
            } else
            {
                currObjective = AiObjective.idle;
            }

            destination = new Vector2(targetSumo.transform.position.x, targetSumo.transform.position.z);
        }

        float angleTowardsClosestSumo = rotTowardsTarget.eulerAngles.y;
        this.rotateToY = angleTowardsClosestSumo;
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
            return sumos[0];
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
        return GetClosestGameObject(sumos.FindAll(s => s.isPushing).ConvertAll(s => s.gameObject), transform.position).GetComponent<Sumo>();
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
