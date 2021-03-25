using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JeroAI : SumoBaseAI
{
    public GameHandler gameHandler;

    //PoweUp vars
    private GameObject pointPower;

    //Player vars
    private List<SumoBaseAI> allEnemySumos;
    private GameObject closestSumo;
    private float mypoints;
    
    //Ring vars
    private GameObject ring;
    private float ringDistance;

    //Rotation vars
    private bool ToRing = true;
    private bool ToPlayer;
    private bool ToPowerUp;

    // Update is called once per frame
    void Update()
    {
        if (!gameHandler.gameStopped)
        {
            GetRing();
            GetSumo();
            GetPowerUps();

            if (mypoints < 48)
            {
                if (pointPower)
                {
                    ObjectivePowerUp(pointPower);
                }
                else
                {
                    if (ringDistance < 3f) //ring collider bounds not working --> if(ring.bounds.Contains(transform.position)) or if(ring.bounds.extents < X )
                    {
                        if (closestSumo.GetComponent<Sumo>().isPushing == true && !pointPower)
                        {
                            ObjectiveSelf();
                        }
                        else if (closestSumo.GetComponent<Sumo>().actionLocked == true && !pointPower)
                        {
                            ObjectivePlayer(closestSumo);
                        }
                    }
                    else
                    {
                        ObjectiveRing(ring);
                    }
                }
            }
            else 
            {
                ObjectiveRing(ring);
            }
            
            Rotation();          
        }       
    }

    //Rotate towards the target (ring, player or powerUp)
    void Rotation()
    {
        if (ToRing)
        {
            this.rotateToY = Quaternion.LookRotation(ring.transform.position - transform.position).eulerAngles.y;
        }
        else if (ToPlayer && closestSumo)
        {
            this.rotateToY = Quaternion.LookRotation(closestSumo.transform.position - transform.position).eulerAngles.y;
        }
        else if (ToPowerUp && pointPower)
        {
            this.rotateToY = Quaternion.LookRotation(pointPower.transform.position - transform.position).eulerAngles.y;
        }
    }

    //Set ring as target rotation and destination + walk to ring
    void ObjectiveRing(GameObject ring)
    {
        ToRing = true;
        ToPlayer = false;
        ToPowerUp = false;

        this.currObjective = AiObjective.idle;
        this.destination = new Vector2(ring.transform.position.x, ring.transform.position.z);
    }

    //Set a player as the target
    void ObjectivePlayer(GameObject sumo)
    {
        ToRing = false;
        ToPlayer = true;
        ToPowerUp = false;

        StartCoroutine(DelayedPush(sumo));
    }

    //Set a powerUp as the target
    void ObjectivePowerUp(GameObject powerUp)
    {
        ToRing = false;
        ToPlayer = false;
        ToPowerUp = true;

        StartCoroutine(DelayedPush(powerUp));
    }

    //Stay in place and dodge, take care of yourself :)
    void ObjectiveSelf()
    {
        this.currObjective = AiObjective.dodge;
        this.destination = new Vector2(transform.position.x, transform.position.z);
    }

    //Get object ring and its distance from the AI
    void GetRing()
    { 
        ring = GameObject.Find("ObjectiveCircle");  

        ringDistance = Vector3.Distance(ring.transform.position, transform.position);
    }

    //Get all AI's except self & find the closest one (also gets my AI's current points)
    void GetSumo()
    {
        allEnemySumos = GameObject.FindObjectsOfType<SumoBaseAI>().ToList();
        allEnemySumos.Remove(this);

        closestSumo = FindClosestSumo(allEnemySumos).gameObject;

        mypoints = GetComponent<Points>().points;
    }

    //Sort AI's by distance to self and return closest one (inspired by EyalAI)
    SumoBaseAI FindClosestSumo(List<SumoBaseAI> sumos)
    {
        sumos.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a.transform.position, transform.position);
            float distB = Vector3.Distance(b.transform.position, transform.position);
            return distA.CompareTo(distB);
        });
        if (sumos.Count > 0)
            return sumos[0];
        return null;
    }

    //Get powerUps of type PointBundle if they exists
    void GetPowerUps()
    {
        if (FindObjectOfType<PowerUpPointBundle>() != null)
        {
            pointPower = FindObjectOfType<PowerUpPointBundle>().gameObject;
        }
    }

    //Set destination to target & objective to push (delay before and after the push so the action and rotation are smoother)
    IEnumerator DelayedPush(GameObject dest) 
    {
        this.destination = new Vector2(dest.transform.position.x, dest.transform.position.z);

        yield return new WaitForSeconds(1.5f);

        this.currObjective = AiObjective.push;

        yield return new WaitForSeconds(2f);

        this.currObjective = AiObjective.idle;

    }
}
