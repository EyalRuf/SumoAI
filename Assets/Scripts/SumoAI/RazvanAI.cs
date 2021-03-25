﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazvanAI : SumoBaseAI
{
    public List<Sumo> oSumo;
    List<Powerup> powerUps;

    public Transform ring;
    Transform trans = null;

    Vector2 ringCenter;

    float playerRadius = 3f;
    float pUpRadius = 5f;
    float dist;
    public float power;

    bool isAggressive;
    bool isDeffensive;
    bool pushed;

    protected override void Start()
    {
        base.Start();
        ringCenter = new Vector2(ring.position.x, ring.position.z);
        destination = ringCenter;
        powerUps = new List<Powerup>(FindObjectsOfType<Powerup>());
    }

    private void Update()
    {
        Quaternion rotTowardsRing = Quaternion.LookRotation(ring.transform.position - transform.position);
        float angleTowardsRing = rotTowardsRing.eulerAngles.y;
        this.rotateToY = angleTowardsRing;

        checkIfFirst();

        /*if checkIfFirst() returns true, isDeffensive becomes true, 
         * if false, isAggressive becomes true*/
        if (checkIfFirst())
        {
            isDeffensive = true;
            isAggressive = false;
        } else
        {
            isDeffensive = false;
            isAggressive = true;
        }

        getClosestSumoDist();
        Debug.Log(getClosestSumo());

        //Saves dist between player and closest sumo
        //float dist = Vector3.Distance(this.transform.position, trans.transform.position);

        Sumo sumoWithmostPoints = this.displaySumoWithMostPoints();

        if (isAggressive)
        {
            Quaternion rotTowards = Quaternion.LookRotation(sumoWithmostPoints.transform.position - transform.position);
            float angle = rotTowards.eulerAngles.y;
            this.rotateToY = angle;
            destination = new Vector2(sumoWithmostPoints.transform.position.x, sumoWithmostPoints.transform.position.z);

            dist = Vector3.Distance(this.transform.position, sumoWithmostPoints.transform.position);

            if(dist < playerRadius && isPushing == false)
            {
                Push();
            }
        }

        if (isDeffensive)
        {
            //rotates towards the closest sumo
            Quaternion rotTowardsSumo = Quaternion.LookRotation(trans.transform.position - transform.position);
            float angleTowards = rotTowardsSumo.eulerAngles.y;
            this.rotateToY = angleTowards;

            dist = Vector3.Distance(this.transform.position, trans.transform.position);

            if (this.getClosestSumo().isPushing && this.isDodging == false && dist < playerRadius)
            {
                Dodge();
            }

            // aici ma duc dupa powerups daca am unu aproape
            /*Transform closestPowerUp = getClosestPointsDistance();// saves t
            float myPnts = GetComponent<Points>().points;
            Debug.Log(getClosestPointsDistance());*/

            /*float distanceUntillRing = Vector3.Distance(this.transform.position, ring.transform.position);
            float distanceUntillPowerUp = Vector3.Distance(this.transform.position, closestPowerUp.transform.position);*/

            /*if (myPnts - 2 > displaySumoWithMostPoints().GetComponent<Points>().points)
            {
                if(distanceUntillRing < distanceUntillPowerUp)
                {
                    destination = distanceUntillPowerUp.position;
                } else
                {
                    destination = ringCenter;
                }
            }*/
            
        }

    }

    private Transform getClosestPointsDistance()
    {

        float nearestPU = Mathf.Infinity;
        Vector3 curPos = transform.position;
        Transform t = null;

        foreach(Powerup pUp in powerUps)
        {
            float d = Vector3.Distance(pUp.transform.position, curPos);
            if(d < nearestPU)
            {
                t = pUp.transform;
                nearestPU = d;
            }
        }

        return t;
    }

    /*Powerup closestPowerUp()
    {

    }*/

    //Returns the sumo with most points
    private Sumo displaySumoWithMostPoints()
    {
        int maxVal = 0;
        Sumo auxSumo = oSumo[0];

        foreach(Sumo s in oSumo)
        {
            int curVal = s.GetComponent<Points>().points;
            if(curVal > maxVal)
            {
                maxVal = curVal;
                auxSumo = s;
            }

        }

        return auxSumo;
    }

    //checks if this sumo has more points than the other sumo with most points
    private bool checkIfFirst()
    {
        int myPoints = this.GetComponent<Points>().points;
        int abS = this.displaySumoWithMostPoints().GetComponent<Points>().points;
        
        if(myPoints >= abS)
        {
            return true;
        } else
        {
            return false;
        }
    }

    //Returns closest sumo's dist
    private Transform getClosestSumoDist()
    {
        float nearestDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Sumo other in oSumo)
        {
            float dist = Vector3.Distance(other.transform.position, currentPos);
            if(dist < nearestDist)
            {
                trans = other.transform;
                nearestDist = dist;
            }
        }

        return trans;
    }

    //Returns closest sumo
    Sumo getClosestSumo()
    {
        float nearestDist = Mathf.Infinity;
        Sumo closestSumo = null;
        Vector3 currentPos = transform.position;

        foreach (Sumo other in oSumo)
        {
            float dist = Vector3.Distance(other.transform.position, currentPos);
            if (dist < nearestDist)
            {
                closestSumo = other;
                nearestDist = dist;
            }
        }

        return closestSumo;
    }

    private void breakFree()
    {
        //Vreau ceva care sa impinga tot in juru playerului meu

        Vector3 breakFreePos = this.transform.position;
        Collider[] colliders = Physics.OverlapSphere(breakFreePos, playerRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && this.isPushing == false)
            {
                rb.AddExplosionForce(power, breakFreePos, playerRadius, 3.0f);
            } 
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Obstacle"))
        {
            breakFree();
            Debug.Log("BreakFree");
            //Retreat
        }
    }

}

/*Ideeas
     * in start:    - find center coordinates
     *              - get oSumo coordinates
     *              - 
     *
     * 
     * FindObjectOfType<Powerup>()
     * if this.sumo has most points => get some powerups
     * else play aggressive/deffensive
     * 
     * Requirements:    - function to get oSumo's points
     *                  - what is the distance between the this.Sumo and oSumo?
     *                  - which oSumo is nearest to this.Sumo
     *                  - oSumo is pushing?
     */

/*if (Vector3.Distance(this.transform.position, trans.transform.position) < 5f)
    {
        Push();
    }

    if (Vector3.Distance(this.transform.position, trans.transform.position) < 5f)
    {
        Dodge();
    }*/

/*if sumo is in center, this.sumo push
 * if this.sumo is in center -> dodge

 * if closest sumo with most points within range ->rotate towards 
 *                                              ->push outside of circle
 * if this.sumo has most points defend -> dodge
 *                                     -> pick up powerups

 */
