using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JeroAI : SumoBaseAI
{
    //PoweUp vars
    private List<GameObject> pointPowerUps;
    private GameObject pointPower;
    private GameObject closestPointPowerUp;

    //Player vars
    private List<SumoBaseAI> allEnemySumos;
    private GameObject closestSumo;

    //Ring vars
    private GameObject ring;
    private float ringDistance;

    //Rotation vars
    private bool ToRing;
    private bool ToPlayer;
    private bool ToPowerUp;

    // Update is called once per frame
    void Update()
    {
        GetRing();
        GetSumo();
        

        if (ringDistance < 5f) //ring bounds to be implemented?
        {
            if (closestSumo && Vector3.Distance(closestSumo.transform.position, transform.position) < 1f)
            {
                ObjectivePlayer(closestSumo);
            }

            GetPowerUps();
            
        }
        else
        {
            ObjectiveRing(ring);
        }

        foreach (SumoBaseAI sumo in allEnemySumos)
        {
            if (Vector3.Distance(sumo.transform.position, transform.position) < 2f)
            {
                closestSumo = sumo.gameObject;
            }
        }

        if (ToRing)
        {
            this.rotateToY = Quaternion.LookRotation(ring.transform.position - transform.position).eulerAngles.y;
            Debug.Log("Rotating to ring");
        }
        else if (ToPlayer)
        {
            this.rotateToY = Quaternion.LookRotation(closestSumo.transform.position - transform.position).eulerAngles.y;
            Debug.Log("Rotating to player");
        }
        else if (ToPowerUp)
        {
            this.rotateToY = Quaternion.LookRotation(closestPointPowerUp.transform.position - transform.position).eulerAngles.y;
        }
    }

    void ObjectiveRing(GameObject ring)
    {
        Debug.Log("ObjectiveRing");
        ToRing = true;
        ToPlayer = false;
        ToPowerUp = false;

        StartCoroutine(DelayedPush(ring));
    }

    void ObjectivePlayer(GameObject sumo)
    {
        ToRing = false;
        ToPlayer = true;
        ToPowerUp = false;

        StartCoroutine(DelayedPush(sumo));
    }

    void ObjectivePowerUp(GameObject powerUp)
    {
        ToRing = false;
        ToPlayer = false;
        ToPowerUp = true;

        StartCoroutine(DelayedPush(powerUp));
    }

    void ObjectiveSelf()
    {
        Debug.Log("ObjectiveSelf");
        if (closestSumo.GetComponent<Sumo>().isPushing)
        {
            this.currObjective = AiObjective.dodge;
        }
        this.destination = new Vector2(transform.position.x, transform.position.z);
    }

    void GetRing()
    { 
        ring = GameObject.Find("ObjectiveCircle");  

        ringDistance = Vector3.Distance(ring.transform.position, transform.position);
    }

    void GetSumo()
    {
        allEnemySumos = GameObject.FindObjectsOfType<SumoBaseAI>().ToList();
        allEnemySumos.Remove(this);
    }

    void GetPowerUps()
    {
        pointPower = FindObjectOfType<PowerUpPointBundle>()?.gameObject;
        if(pointPower != null && pointPowerUps.Contains(pointPower))
        {
            pointPowerUps.Add(pointPower);
            foreach (GameObject power in pointPowerUps)
            {
                if (Vector3.Distance(pointPower.transform.position, transform.position) < 1f)
                {
                    closestPointPowerUp = power;
                }
            }
        }
    }

    IEnumerator DelayedPush(GameObject dest) 
    {
        this.destination = new Vector2(dest.transform.position.x, dest.transform.position.z);
        this.currObjective = AiObjective.idle;

        yield return new WaitForSeconds(2f);

        this.currObjective = AiObjective.push;

        closestSumo = null;

        yield return new WaitForSeconds(0.1f);

        this.currObjective = AiObjective.idle;
    }
}
