using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustinAI : SumoBaseAI
{

    //Variables
    List<Sumo> sumoList;
    List<GameObject> obstacleList;
    Transform sumoArena;
    float radius;

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

        Vector3 sumoArenaDistance = sumoArena.position - transform.position;

        float sumoArenaDistanceValue = sumoArenaDistance.sqrMagnitude;
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
                Vector2 obstaclePushDestination = 1.2f * sumoPointsTarget().transform.position - obstacleFinder().transform.position;
                this.destination = obstaclePushDestination;
                Quaternion rotation = Quaternion.LookRotation(obstaclePushDestination);
                float sumoLookAngle = rotation.eulerAngles.y;
                this.rotateToY = sumoLookAngle;
                currObjective = AiObjective.push;
            }
        }
        else
        {
            if(powerupFinder() != null && powerupFinder().GetComponent<PowerUpPointBundle>() != null)
            {
                //run for points
                Vector2 centerDestination = new Vector2(powerupFinder().transform.position.x, powerupFinder().transform.position.z);
                this.destination = centerDestination;
                Quaternion rotation = Quaternion.LookRotation(centerDestination);
                float sumoLookAngle = rotation.eulerAngles.y;
                this.rotateToY = sumoLookAngle;
                currObjective = AiObjective.idle;
            }
            else
            {
                //Push into center
                Vector2 centerDestination = new Vector2(sumoArena.position.x, sumoArena.position.z);
                this.destination = centerDestination;
                Quaternion rotation = Quaternion.LookRotation(centerDestination);
                float sumoLookAngle = rotation.eulerAngles.y;
                this.rotateToY = sumoLookAngle;
                currObjective = AiObjective.push;
            }
        }
        //Debug.Log("closest sumo " + nearestSumoAI());
    }

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
