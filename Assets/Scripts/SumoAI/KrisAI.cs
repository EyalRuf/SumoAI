using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KrisAI : SumoBaseAI
{
    //Stored Variables
    List<Sumo> sumos;
    Transform ring;

    //Change the distance at which it dodges away from opponents

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
        Sumo closestSumo = FindClosestSumo();
        Sumo groupedSumo = FindGroupedSumo();
        Powerup closestPUp = FindClosestPowerUp();

        Debug.Log(closestPUp);

        DecisionMaking(closestSumo,groupedSumo,closestPUp,ring);
        
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
        // Is there a particular reason why you're looking for all sumos again after you've found them in Start?
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

    private void DecisionMaking(Sumo closestSumo, Sumo groupedSumo, Powerup closestPowerUp, Transform objectiveCircle)
    {
        // 3 seems like an arbitrary (magic) number, try to refrain from using magic numbers
        if ((closestSumo.transform.position - transform.position).sqrMagnitude < 3)
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
        else if (closestPowerUp != null && closestPowerUp.GetComponent<PowerUpPointBundle>() != null)
        {
            currObjective = AiObjective.dodge;
            Vector2 dest = new Vector2(closestPowerUp.transform.position.x, closestPowerUp.transform.position.z);
            Quaternion rotationTowards = Quaternion.LookRotation(closestPowerUp.transform.position);
            float angleTowards = rotationTowards.eulerAngles.y;

            this.destination = dest;
            this.rotateToY = angleTowards;
        }
        else
        {
            Vector2 dest = new Vector2(ring.transform.position.x, ring.transform.position.z);
            Quaternion rotationTowards = Quaternion.LookRotation(ring.transform.position);
            float angleTowards = rotationTowards.eulerAngles.y;

            this.destination = dest;
            this.rotateToY = angleTowards;
            currObjective = AiObjective.idle;
        }
    }

}
