using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazvanAI2 : SumoBaseAI
{
    public List<Sumo> otherSumo;

    public Transform ring;
    Transform trans = null;

    Vector2 ringCenter;

    float playerRadius = 3f;
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
    }

    private void Update()
    {
        Quaternion rotTowardsRing = Quaternion.LookRotation(ring.transform.position - transform.position);
        float angleTowardsRing = rotTowardsRing.eulerAngles.y;
        this.rotateToY = angleTowardsRing;
    }

    private Sumo displaySumoWithMostPoints()
    {
        int maxVal = 0;
        // Why is it named oSumo?
        Sumo auxSumo = otherSumo[0];

        foreach (Sumo s in otherSumo)
        {
            // Maybe you can cache the points variable on start, this way you can prevent unnecessary points GetComponent code
            int curVal = s.GetComponent<Points>().points;
            if (curVal > maxVal)
            {
                maxVal = curVal;
                auxSumo = s;
            }

        }

        return auxSumo;
    }

    private bool checkIfFirst()
    {
        // Maybe you can cache the points variable on start, this way you can prevent unnecessary points GetComponent code
        int myPoints = this.GetComponent<Points>().points;
        int abS = this.displaySumoWithMostPoints().GetComponent<Points>().points;

        if (myPoints >= abS)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Transform getClosestSumoDist()
    {
        float nearestDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Sumo other in otherSumo)
        {
            float dist = Vector3.Distance(other.transform.position, currentPos);
            if (dist < nearestDist)
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

    private void breakFree()
    {
        Vector3 breakFreePos = this.transform.position;
        Collider[] colliders = Physics.OverlapSphere(breakFreePos, playerRadius);

        // I know foreach is leads to better readability, but it's a little bit less cheaper than for loops
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
