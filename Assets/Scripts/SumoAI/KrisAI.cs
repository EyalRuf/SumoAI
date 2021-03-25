using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KrisAI : SumoBaseAI
{
    //Stored Variables
    List<Sumo> sumos;
    public Transform objectiveCircle;

    //Change the distance at which it dodges away from opponents

    protected override void Start()
    {
        base.Start();
        sumos = new List<Sumo>(FindObjectsOfType<Sumo>());
        sumos.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        Sumo closestSumo = FindClosestSumo();
        Sumo groupedSumo = FindGroupedSumo();
        Powerup closestPUp = FindClosestPowerUp();        
        if ((closestSumo.transform.position - transform.position).sqrMagnitude < 7)
        {
            if (closestSumo == groupedSumo && !isPushing)
            {
                currObjective = AiObjective.push;
                // get grouped sumo pos & angle towards
                Vector2 dest = new Vector2(closestSumo.transform.position.x, closestSumo.transform.position.z);
                Quaternion rotationAway = Quaternion.Inverse(closestSumo.transform.rotation);
                float angleAway = rotationAway.eulerAngles.y;

                this.destination = dest;
                this.rotateToY = angleAway;
            }
            else if (!isDodging)
            {
                currObjective = AiObjective.dodge;
            }
            else
            {
                currObjective = AiObjective.idle;
            }
        }
        else if (closestPUp != null && closestPUp.name == "PowerUpPointBundle")
        {
            currObjective = AiObjective.dodge;
            Vector2 dest = new Vector2(closestPUp.transform.position.x, closestPUp.transform.position.z);
            Quaternion rotationTowards = Quaternion.LookRotation(closestPUp.transform.position);
            float angleTowards = rotationTowards.eulerAngles.y;

            this.destination = dest;
            this.rotateToY = angleTowards;
        }
        else
        {
            currObjective = AiObjective.idle;
        }
    }

    private Sumo FindClosestSumo()
    {

        Sumo closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (Sumo sumo in sumos)
        {
            if (sumo.Equals(this.gameObject))
                continue;
            Vector3 diff = sumo.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = sumo;
                distance = curDistance;
            }
        }
        return closest;
    }

    //Find a sumo that's the closest to all sumos - center of the group
    private Sumo FindGroupedSumo()
    {
        Sumo[] gos1;
        gos1 = FindObjectsOfType<SumoBaseAI>();

        Sumo grouped = null;
        float groupDistance = Mathf.Infinity;

        foreach (Sumo go1 in gos1)
        {
            if (go1.Equals(this.gameObject))
                continue;
            Sumo[] gos2;
            gos2 = FindObjectsOfType<SumoBaseAI>();
            float sumDistance = 0;
            foreach (Sumo go2 in gos2)
            {
                Vector3 diff = go2.transform.position - go1.transform.position;
                float curDistance = diff.sqrMagnitude;
                sumDistance += curDistance;
            }
            if (groupDistance > sumDistance)
            {
                groupDistance = sumDistance;
                grouped = go1;
            }
        }
        return grouped;
    }

    private Powerup FindClosestPowerUp()
    {
        Powerup[] powerups;
        powerups = FindObjectsOfType<Powerup>();

        Powerup closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (Powerup pu in powerups)
        {
            if (pu.Equals(this.gameObject))
                continue;
            Vector3 diff = pu.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = pu;
                distance = curDistance;
            }
        }
        return closest;
    }

    private GameObject DecisionMaking(Sumo closestSumo, Sumo groupedSumo, Powerup closestPowerup)
    {

        return null;
    }

}
