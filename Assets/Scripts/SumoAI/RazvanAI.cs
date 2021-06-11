using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazvanAI : SumoBaseAI
{
    private enum Strategy
    {
        defensive,
        aggressive
    }

    public List<Sumo> otherSumo;

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
    bool spawned = false;

    private Strategy state;

    protected override void Start()
    {
        base.Start();
        ringCenter = new Vector2(ring.position.x, ring.position.z);
        myPoints = this.gameObject.GetComponent<Points>().points;
        //destination = ringCenter;
    }

    private void Update()
    {
        Transform closestSumoPosition = this.getClosestSumoDist();
        Sumo closesstSumo = this.getClosestSumo();
        Sumo sumoWithmostPoints = this.displaySumoWithMostPoints();

        Quaternion rotTowardsRing = Quaternion.LookRotation(ring.transform.position - transform.position);
        float angleTowardsRing = rotTowardsRing.eulerAngles.y;
        //this.rotateToY = angleTowardsRing;

        checkIfFirst();

        /*if checkIfFirst() returns true, isDeffensive becomes true, 
         * if false, isAggressive becomes true*/
        
        if (checkIfFirst())
        {
            state = Strategy.defensive;
        } else
        {
            state = Strategy.aggressive;
        }

        Quaternion rotTowards = Quaternion.LookRotation(sumoWithmostPoints.transform.position - transform.position);

        strategyDecision(state, closestSumoPosition, sumoWithmostPoints);
    }

    void strategyDecision(Strategy state, Transform nearSumo, Sumo sumoWithHighestScore)
    {
        Powerup[] powerUps;
        powerUps = FindObjectsOfType<Powerup>();
        Vector3 curPos = transform.position;
        //Powerup p = null;
        Powerup pointPU = null;

        if (state == Strategy.defensive)
        {
            Quaternion rotTowardsNearSumo = Quaternion.LookRotation(nearSumo.position - transform.position);
            float angleTowardsNearSumo = rotTowardsNearSumo.eulerAngles.y;
            this.rotateToY = angleTowardsNearSumo;

            dist = Vector3.Distance(this.transform.position, trans.transform.position);

            if (this.getClosestSumo().isPushing && this.isDodging == false && dist < playerRadius)
            {
                Dodge();
            }

            if(myPoints < 48)
            {
                PowerUpAction(powerUps, pointPU, "PowerUp");
            }

            Debug.Log("defensive");
        }

        if (state == Strategy.aggressive)
        {
            Quaternion rotTowardsSumoWithMostPoints = Quaternion.LookRotation(sumoWithHighestScore.transform.position - transform.position);
            float angleTowardsSumoWithMosrPoints = rotTowardsSumoWithMostPoints.eulerAngles.y;
             if(spawned == false)
            {
                this.rotateToY = angleTowardsSumoWithMosrPoints;
                if (dist <= playerRadius && isPushing == false && sumoWithHighestScore.isDodging == false)
                {

                    this.rotateToY = angleTowardsSumoWithMosrPoints;
                    StartCoroutine(waitToPush(2f));

                }
            }
            

            //chemi functia 
            PowerUpAction(powerUps, pointPU, "PointsPowerUp");
        }
    }

    private void PowerUpAction(Powerup[] powUP, Powerup storedPU, string powerTag)
    {
        foreach(Powerup pUp in powUP)
        {
            if(pUp.tag == powerTag)
            {
                storedPU = pUp;
                spawned = true;

                float distanceBetweenMeAndPowerUp = Vector3.Distance(this.transform.position, storedPU.transform.position);
                float distanceBetweenClosestSumoAndPowerUp = Vector3.Distance(getClosestSumo().transform.position, storedPU.transform.position);

                Quaternion rotToPointsPu = Quaternion.LookRotation(storedPU.transform.position - transform.position);
                float angleToPointsPu = rotToPointsPu.eulerAngles.y;
                Vector2 dst = new Vector2(storedPU.transform.position.x, storedPU.transform.position.z);

                if(storedPU != null)
                {
                    this.rotateToY = angleToPointsPu;
                    destination = dst;
                    StartCoroutine(waitToPickUp());
                }

                if (distanceBetweenClosestSumoAndPowerUp < distanceBetweenMeAndPowerUp)
                {
                    StartCoroutine(waitToPush(1f));
                }

            }

            if (pickedUp)
            {
                destination = ringCenter;
                storedPU = null;
                pickedUp = false;
            }
        }
    }

    //Returns the sumo with most points
    private Sumo displaySumoWithMostPoints()
    {
        int maxVal = 0;
        Sumo auxSumo = otherSumo[0];

        foreach(Sumo s in otherSumo)
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
        //int myPoints = this.GetComponent<Points>().points;
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

        foreach (Sumo other in otherSumo)
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

        foreach (Sumo other in otherSumo)
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
        spawned = false;
        

    }

    IEnumerator waitToPush(/*Vector2 dest*/ float time/*, float angleToRot*/)
    {
/*        this.destination = dest;*/
        /*this.rotateToY = angleToRot;*/

        yield return new WaitForSeconds(time);

        if (isPushing == false)
        {
            Push();
        }
        
    }
}
