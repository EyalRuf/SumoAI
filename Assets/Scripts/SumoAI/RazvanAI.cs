using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazvanAI : SumoBaseAI
{
    public List<Sumo> oSumo;

    public Transform ring;
    Transform trans = null;

    Vector2 ringCenter;

    float playerRadius = 2f;
    float dist;
    float myPoints;
    public float power;

    bool isAggressive = false;
    bool isDeffensive = false;
    bool pushed;
    bool pickedUp = false;

    protected override void Start()
    {
        base.Start();
        ringCenter = new Vector2(ring.position.x, ring.position.z);
        myPoints = this.gameObject.GetComponent<Points>().points;
        //destination = ringCenter;
    }

    private void Update()
    {
        Quaternion rotTowardsRing = Quaternion.LookRotation(ring.transform.position - transform.position);
        float angleTowardsRing = rotTowardsRing.eulerAngles.y;
        this.rotateToY = angleTowardsRing;

        checkIfFirst();

        /*if checkIfFirst() returns true, isDeffensive becomes true, 
         * if false, isAggressive becomes true*/
        //nu mai face asta, folosesti un enum cu 2 stateuri, defensive si agressive
        //bagi toata sectiunea asta de defensive agresive intr o functie de tip void
        
        if (checkIfFirst())
        {
            isDeffensive = true;
            isAggressive = false;
        } else
        {
            isDeffensive = false;
            isAggressive = true;
        }

        //Salveazaintro variabila
        getClosestSumoDist();

        Sumo sumoWithmostPoints = this.displaySumoWithMostPoints();

        Powerup[] powerUps;
        powerUps = FindObjectsOfType<Powerup>();
        Vector3 curPos = transform.position;
        Powerup p = null;
        Powerup pointPU = null;

        Quaternion rotTowards = Quaternion.LookRotation(sumoWithmostPoints.transform.position - transform.position);

        if (isAggressive)
        {
            float angle = rotTowards.eulerAngles.y;
            this.rotateToY = angle;

            dist = Vector3.Distance(this.transform.position, sumoWithmostPoints.transform.position);

            //Push , if I can push, if the sumo is in my sumo range, if the other sumo is not dodging
            if (dist <= playerRadius && isPushing == false && sumoWithmostPoints.isDodging == false)
            {
                Push();
            }

            //If there is any sumo pushing towards me, dodge when he is in my radius
            /*if (this.getClosestSumo().isPushing && this.isDodging == false && dist < playerRadius)
            {
                Dodge();
            }*/

            foreach (Powerup pUp in powerUps)
            {
                Vector2 thisPos = new Vector2(this.trans.position.x, this.transform.position.z);

                //Check if there is any point power up, if yes, start courutine
                if (pUp.tag == "PointsPowerUp")
                {
                    pointPU = pUp;

                    Quaternion rotToPointsPu = Quaternion.LookRotation(pointPU.transform.position - transform.position);
                    float angleToPointsPu = rotToPointsPu.eulerAngles.y;
                    Vector2 dst = new Vector2(pointPU.transform.position.x, pointPU.transform.position.z);

                    StartCoroutine(waitToPush(dst, 1.5f, angleToPointsPu));

                    destination = dst;
                    StartCoroutine(waitToPickUp());
                }
                //daca distanta intre cel mai apropriat sumo si power up e mai mica decat distanta intre mine si pu, push

                //If there is any point power up, go pick it up
                if (pickedUp)
                {
                    destination = ringCenter;
                    pointPU = null;
                    pickedUp = false;
                }

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

            //If there is any point power up, go pick it up
            foreach (Powerup pUp in powerUps)
            {
                if (pUp.tag == "PowerUp")
                {
                    pointPU = pUp;

                    Quaternion rotToOtherPu = Quaternion.LookRotation(pointPU.transform.position - transform.position);
                    float angleToOtherPU = rotToOtherPu.eulerAngles.y;

                    Vector2 dst = new Vector2(pointPU.transform.position.x, pointPU.transform.position.z);
                    if(myPoints < 45)
                    {
                        StartCoroutine(waitToPush(dst, 1.5f, angleToOtherPU));

                        destination = dst;
                        StartCoroutine(waitToPickUp());
                    }
                }

                if (pickedUp)
                {
                    destination = ringCenter;
                    pointPU = null;
                    pickedUp = false;
                }

            }
        }
    }

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

    //Secret ability
    private void breakFree()
    {
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

    /*if I collide with an obstacle I breakf ree
     * if I collide with a player and I'm in deffensive mode, I can break free
     */
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (isDeffensive)
            {
                breakFree();
            }
        }

        if (collision.collider.CompareTag("Obstacle"))
        {
            breakFree();
        }
    }

    IEnumerator waitToPickUp()
    {
        yield return new WaitForSeconds(6f);
        pickedUp = true;

    }

    IEnumerator waitToPush(Vector2 dest, float time, float angleToP)
    {
        this.destination = dest;
        this.rotateToY = angleToP;

        yield return new WaitForSeconds(time);

        Debug.Log("mere");

        if (isPushing == false)
        {
            Push();
        }
        
    }
}
