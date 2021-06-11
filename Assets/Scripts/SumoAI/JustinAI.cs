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

        //Creating the SumoList, locating the game arena, and stating the radius of the arena that will be used for certain behaviours.
        sumoList = new List<Sumo>(FindObjectsOfType<Sumo>());
        sumoList.Remove(this);

        sumoArena = GameObject.Find("ObjectiveCircle").transform;

        radius = 9.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //Creating variables to be used in the behaviours below.
        sumoPos = new Vector2(transform.position.x, transform.position.z);
        Vector3 sumoArenaDistance = sumoArena.position - transform.position;

        float sumoArenaDistanceValue = sumoArenaDistance.sqrMagnitude;


        //Stay within inner 75% radius of circle.

        if (Mathf.Sqrt(sumoArenaDistanceValue) < 0.75*radius)
        {
            //Dodge if a Sumo is within 2 metres of mine.
            if ((nearestSumoAI().transform.position - transform.position).sqrMagnitude < 2)
            {
                currObjective = AiObjective.dodge;
            }
            //If there is an obstacle nearby, locate it and push it into the currently highest scoring player.
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
        //If my SumoAI is outside the 75% inner radius of the circle.
        else
        {
            //Run for points if powerupFinder has found a points bundle to run to.
            if (powerupFinder() != null && powerupFinder().GetComponent<PowerUpPointBundle>() != null)
            {
                Vector2 centerDestination = new Vector2(powerupFinder().transform.position.x, powerupFinder().transform.position.z);
                this.destination = centerDestination;
                Quaternion rotation = Quaternion.LookRotation(centerDestination - sumoPos);
                float sumoLookAngle = rotation.eulerAngles.y;
                this.rotateToY = sumoLookAngle;
                currObjective = AiObjective.idle;
            }
            //Otherwise, face towards and push into the center of the arena.
            else
            {
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
    }

    //Finds the nearest SUMO using the sumoList and returns its Sumo type object.
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

    //Finds the nearest powerup relative to my SUMO AI using the powerUpsList and returns the powerup type object.
    private Powerup powerupFinder()
    {
        Powerup[] powerUpsList = FindObjectsOfType<Powerup>();
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

    //Finds the SUMO with the highest amount of points using the sumoList and returns the Sumo type object.
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

    //Finds the nearest obstacle relative to my SUMO AI using the obstacleList and returns the obstacle type object.
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
