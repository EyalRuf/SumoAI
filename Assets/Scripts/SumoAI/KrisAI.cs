using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KrisAI : SumoBaseAI
{
    //Stored Variables
    List<Sumo> sumos;
    Transform ring;

    protected override void Start()
    {
        base.Start();

        sumos = new List<Sumo>(FindObjectsOfType<Sumo>());
        sumos.Remove(this);

        ring = GameObject.FindGameObjectWithTag("Ring").transform;
    }

    // Update is called once per frame
    void Update()
    {
        DecisionMaking(FindClosestSumo(),FindGroupedSumo(),FindClosestPowerUp(),ring);        
    }

    //Finds closest sumo
    private Sumo FindClosestSumo()
    {

        Sumo closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        /*
         * 
         * Goes through the list made in the start function 
         * and searches for the sumo with least distance to itself
         * 
         */
        foreach (Sumo sumo in sumos)
        {
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

    //Finds a sumo that's the closest to all sumos - center of the group
    private Sumo FindGroupedSumo()
    {
        Sumo grouped = null;
        float groupDistance = Mathf.Infinity;
        /*
         * 
         *   Goes through each sumo to detect which one has the least distance to all other sumos, 
         *   defines that one as the center of the group
         *
         */
        foreach (Sumo sumo in sumos)
        {
            Sumo[] gos2;
            gos2 = FindObjectsOfType<SumoBaseAI>();
            float sumDistance = 0;
            foreach (Sumo go2 in gos2)
            {
                Vector3 diff = go2.transform.position - sumo.transform.position;
                float curDistance = diff.sqrMagnitude;
                sumDistance += curDistance;
            }
            if (groupDistance > sumDistance)
            {
                groupDistance = sumDistance;
                grouped = sumo;
            }
        }
        return grouped;
    }

    //Finds the closest power up object to itself similair to the sumo function
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

    //Calculates the angle towards a desired location
    private float CalculateAngle(Transform transform)
    {
        Quaternion rotation = Quaternion.LookRotation(transform.transform.position);
        float angle = rotation.eulerAngles.y;
        return angle;
    }

    //Calculates the destination vector for the ai to travel with
    private Vector2 CalculateDestination(Transform transform)
    {
        Vector2 dest = new Vector2(transform.position.x, transform.position.z);
        return dest;
    }

    //Makes all the decisions
    private void DecisionMaking(Sumo closestSumo, Sumo groupedSumo, Powerup closestPowerUp, Transform objectiveCircle)
    {
        //If the closest sumo is closer than 3 units away do the following
        if ((closestSumo.transform.position - transform.position).sqrMagnitude < 3) 
        {
            /*
             * if the closest sumo is the grouped sumo (=> if my Ai is closest to the center of the group)
             * and I can push then push away from the group
             */
            if (closestSumo == groupedSumo && !isPushing) 
            {
                currObjective = AiObjective.push;
                this.destination = CalculateDestination(closestSumo.transform);
                this.rotateToY = -CalculateAngle(closestSumo.transform);
            }
            else if (!isDodging) //otherwise if i can dodge then dodge
            {
                currObjective = AiObjective.dodge;
            }
            else // otherwise do nothing
            {
                currObjective = AiObjective.idle;
            }
        }
        //if the closest powerup to me is the points bundle run for it
        else if (closestPowerUp != null && closestPowerUp.GetComponent<PowerUpPointBundle>() != null) 
        {
            currObjective = AiObjective.dodge;
            this.destination = CalculateDestination(closestPowerUp.transform);
            this.rotateToY = CalculateAngle(closestPowerUp.transform); ;
        }
        else // if all else fails come back to the ring center
        {
            currObjective = AiObjective.idle;
            this.destination = CalculateDestination(ring.transform);
            this.rotateToY = CalculateAngle(ring.transform);
        }
    }
}