using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustinAI : SumoBaseAI
{

    //Variables and Lists

    List<Sumo> sumoList;
    List<GameObject> obstacleList;
    Transform sumoArena;
    float radius;
    Vector2 sumoPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        sumoList = new List<Sumo>(FindObjectsOfType<Sumo>());
        sumoList.Remove(this);

        sumoArena = GameObject.Find("ObjectiveCircle").transform;

        radius = 9.5f;
    }

    // Update is called once per frame
    void Update()
    {
        sumoPos = new Vector2(transform.position.x, transform.position.z);
        Vector3 sumoArenaDistance = sumoArena.position - transform.position;

        float sumoArenaDistanceValue = sumoArenaDistance.sqrMagnitude;


        //Stay within inner 75% radius of circle.

        if (Mathf.Sqrt(sumoArenaDistanceValue) < 0.75*radius)
        {
            //Debug.Log("75% of radius reached");
            if ((nearestSumoAI().transform.position - transform.position).sqrMagnitude < 2)
            {
                currObjective = AiObjective.dodge;
                ///Debug.Log("dodge");
            }

            else if(obstacleFinder() != null && sumoPointsTarget() != null)
            {
                Vector3 obstaclePushDestination = 1.2f * sumoPointsTarget().transform.position - obstacleFinder().transform.position;
                this.destination = obstaclePushDestination;
                Quaternion rotation = Quaternion.LookRotation(obstacleFinder().transform.position - transform.position);
                float sumoLookAngle = rotation.eulerAngles.y;
                this.rotateToY = sumoLookAngle;
                if(transform.position == obstaclePushDestination)
                {
                    currObjective = AiObjective.push;
                }
                else
                {
                    currObjective = AiObjective.idle;
                }
            }
        }
        else
        {
            if(powerupFinder() != null && powerupFinder().GetComponent<PowerUpPointBundle>() != null)
            {
                //Run for points if powerupFinder has found a points bundle to run to.
                Vector2 centerDestination = new Vector2(powerupFinder().transform.position.x, powerupFinder().transform.position.z);
                this.destination = centerDestination;
                Quaternion rotation = Quaternion.LookRotation(centerDestination - sumoPos);
                float sumoLookAngle = rotation.eulerAngles.y;
                this.rotateToY = sumoLookAngle;
                currObjective = AiObjective.idle;
            }
            else
            {
                //Otherwise, face towards and push into the center of the arena.
                Vector2 centerDestination = new Vector2(sumoArena.position.x, sumoArena.position.z);
                this.destination = centerDestination;
                Quaternion rotation = Quaternion.LookRotation(sumoArena.transform.position - transform.position);
                float sumoLookAngle = rotation.eulerAngles.y;
                this.rotateToY = sumoLookAngle;
                if(transform.rotation.y == sumoLookAngle)
                {
                    currObjective = AiObjective.push;
                }
                else
                {
                    currObjective = AiObjective.idle;
                }
            }
        }
        //Debug.Log("closest sumo " + nearestSumoAI());
    }

    //Function that finds the nearest SUMO using the sumoList and returns it's name.
    private Sumo nearestSumoAI()
    {
        Sumo nearestSumo = null;
        float sumoDistance = Mathf.Infinity;

        foreach (Sumo sumo in sumoList)
        {
            Vector3 distance = sumo.transform.position - transform.position;
            float distanceValue = distance.sqrMagnitude;
            if (distanceValue < sumoDistance)
            {
                sumoDistance = distanceValue;
                nearestSumo = sumo;
            }

        }

        return nearestSumo;
    }

    //Function that finds the nearest powerup using the powerUpsList and returns its positional data relative to my SUMO AI.
    private Powerup powerupFinder()
    {
        Powerup[] powerUpsList = FindObjectsOfType<Powerup>();



        //new List<Powerup>((IEnumerable<Powerup>)FindObjectOfType<Powerup>());

        Powerup nearestPowerUp = null;
        float powerUpDistance = Mathf.Infinity;

        foreach (Powerup powerUp in powerUpsList)
        {
            Vector3 distance = powerUp.transform.position - transform.position;
            float distanceValue = distance.sqrMagnitude;
            if (distanceValue < powerUpDistance)
            {
                powerUpDistance = distanceValue;
                nearestPowerUp = powerUp;
            }

        }

        return nearestPowerUp;
    }

    //Function that finds the SUMO with the highest amount of points using the sumoList and returns that name.
    private Sumo sumoPointsTarget()
    {
        Sumo sumoHighestPoints = null;
        float sumoPoints = 0;

        foreach (Sumo sumo in sumoList)
        {
            float pointsValue = sumo.GetComponent<Points>().points;
            if (pointsValue > sumoPoints)
            {
                sumoPoints = pointsValue;
                sumoHighestPoints = sumo;
            }

        }

        return sumoHighestPoints;
    }

    //Function that finds the nearest obstacle using the obstacleList and return its positional data relative to my SUMO AI.
    private GameObject obstacleFinder()
    {
        obstacleList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Obstacle"));

        GameObject nearestObstacle = null;
        float sumoDistance = Mathf.Infinity;

        foreach (GameObject obstacle in obstacleList)
        {
            Vector3 distance = obstacle.transform.position - transform.position;
            float distanceValue = distance.sqrMagnitude;
            if (distanceValue < sumoDistance)
            {
                sumoDistance = distanceValue;
                nearestObstacle = obstacle;
            }
            
        }

        return nearestObstacle;
    }
}
