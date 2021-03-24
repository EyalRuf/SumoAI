using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichardAI : MonoBehaviour
{

    /// <summary>
    /// Rename some of these variables
    /// Clean up powerup code
    /// Add pointbundle check
    /// Add players around powerup check
    /// </summary>

    [Header("Settings")]
    [SerializeField] float powerUpSearchRadius;
    [SerializeField] float ForgetPowerUpDistance;

    //Sumo variables
    SumoBaseAI baseAI;
    List<GameObject> otherSumos = new List<GameObject>();

    //Ring variables
    GameObject ring;
    float ringRadius;
    float GetInsideRingRadius;

    enum RingState { inside, closeInside, outside };
    RingState ringState;

    enum ActionState { evade, returnToRing, attack, gettingPowerup};
    ActionState actionState;

    //Targets
    GameObject powerUpTarget;
    GameObject attackTarget;

    //Target Attack variables
    Vector3 targetFirstFrame;
    Vector3 targetSecondFrame;
    Vector3 targetMovement;

    //Evasion variables
    List<Vector3> sumoFirstFrame = new List<Vector3>(); 
    List<Vector3> sumoSecondFrame = new List<Vector3>();
    bool hasDodged;
    int frameCounter;

    void Start()
    {
        baseAI = GetComponent<SumoBaseAI>();

        //All sumos need a points script, so that's how I find them. There could be an AI without a tag or the right name, so this is a safer way of checking for players.
        var sumos = FindObjectsOfType<Points>();

        foreach (var sumo in sumos)
        {
            otherSumos.Add(sumo.gameObject);
        }

        //otherSumos.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        otherSumos.Remove(gameObject);

        //Just to be sure, I check for all possibilities, because the 6 of us can edit the game. The code has got to be flexible for it.
        if (GameObject.FindGameObjectWithTag("Ring"))
        {
            ring = GameObject.FindGameObjectWithTag("Ring");
        }
        else if (GameObject.Find("Ring"))
        {
            ring = GameObject.Find("Ring");
        }
        else if (GameObject.Find("ring"))
        {
            ring = GameObject.Find("ring");
        }

        ringRadius = ring.transform.localScale.z / 2; //Get the radius of the ring
        GetInsideRingRadius = ringRadius * 0.7f; //Make the radius smaller to pursuade the AI to get deeper into the ring
    }

    void Update()
    {
        //Check for powerups. This is shared across inside and outside ring states, so it happens before anything else
        if (powerUpTarget == null) //Reset the state if the powerup is gone (even if it's grabbed by another player), and look for powerups
        {
            //Check if there's a powerup nearby
            Collider[] objectsFound = Physics.OverlapSphere(transform.position, powerUpSearchRadius);
            List<GameObject> powerUpsFound = new List<GameObject>();
            int powerUpsInMap = 0;

            actionState = ActionState.returnToRing;

            if (objectsFound.Length > 0) //If there are objects within the scanned area
            {
                foreach (var potentialPowerUp in objectsFound) //Checking if any of the scanned objects are powerups
                {
                    if (potentialPowerUp.gameObject.TryGetComponent(out Powerup poweruppy))
                    {
                        powerUpsFound.Add(potentialPowerUp.gameObject);
                        powerUpsInMap++;
                    }
                }

                if (powerUpsInMap > 0) //If there are powerups in the map, and the AI isn't already getting a powerup
                {
                    if (CalculatePriority(powerUpsFound))
                    {
                        actionState = ActionState.gettingPowerup;
                    }
                }
            }
        }
        else if ((transform.position - powerUpTarget.transform.position).sqrMagnitude > ForgetPowerUpDistance * ForgetPowerUpDistance) //Reset the state if the AI is too far away from its target powerup
        {
            actionState = ActionState.returnToRing;
        }

        //More efficient way of checking for distance | (pointA - pointB).sqrMagnitude < distance * distance
        if ((transform.position - ring.transform.position).sqrMagnitude < ringRadius * ringRadius) //If the player is in the outer part of the ring
        {
            ringState = RingState.inside;

            if (actionState != ActionState.gettingPowerup)
                DecideInsideRingAction();

        }
        else //If the player is outside of the ring
        {
            ringState = RingState.outside;

            if (actionState != ActionState.gettingPowerup)
                DecideOutsideRingAction();
        }

        DodgeCalculation();
        TakeAction();
    }

    void TakeAction()
    {
        switch (actionState)
        {
            case ActionState.returnToRing://--TODO: Stop going towards the ring once you are a bit closer to the center--//
                baseAI.destination = new Vector2(ring.transform.position.x, ring.transform.position.z);
                baseAI.rotateToY = Quaternion.LookRotation(ring.transform.position - transform.position).eulerAngles.y;
                break;
            case ActionState.evade: //--TODO: Make the player move unpredictably--//
                baseAI.destination = new Vector2(ring.transform.position.x, ring.transform.position.z);
                baseAI.rotateToY = Quaternion.LookRotation(ring.transform.position - transform.position).eulerAngles.y;
                break;
            case ActionState.attack:
                baseAI.destination = new Vector2(attackTarget.transform.position.x, attackTarget.transform.position.z);
                Attack();
                break;
            case ActionState.gettingPowerup:
                baseAI.destination = new Vector2(powerUpTarget.transform.position.x, powerUpTarget.transform.position.z);
                baseAI.rotateToY = Quaternion.LookRotation(powerUpTarget.transform.position - transform.position).eulerAngles.y;
                break;
            default: //Just go towards the center of the ring for now
                baseAI.destination = new Vector2(ring.transform.position.x, ring.transform.position.z);
                baseAI.rotateToY = Quaternion.LookRotation(ring.transform.position - transform.position).eulerAngles.y;
                break;
        }
    }

    void DecideInsideRingAction()
    {
        //Checking if any other players are inside the ring
        Collider[] playerCheck = Physics.OverlapSphere(ring.transform.position, ringRadius);
        List<GameObject> playersInRing = new List<GameObject>();
        bool targetStillInRing = false;

        for (int i = 0; i < playerCheck.Length; i++)
        {
            if (playerCheck[i].CompareTag("Player") && playerCheck[i].gameObject != gameObject)
            {
                playersInRing.Add(playerCheck[i].gameObject);

                if (playerCheck[i].gameObject == attackTarget) //Check if the target is still in the ring or not
                    targetStillInRing = true;
                else
                    targetStillInRing = false;
            }
        }

        if (!targetStillInRing)
        {
            attackTarget = null;
        }

        if (playersInRing.Count > 0 && !hasDodged) //If there are other players in the ring
        {
            if (attackTarget == null)
            {
                attackTarget = playersInRing[Random.Range(0, playersInRing.Count)]; //Pick a new target
            }

            actionState = ActionState.attack;
        }
        else //If there are no players in the ring
        {
            actionState = ActionState.evade;
        }
    }

    void DecideOutsideRingAction()
    {
        attackTarget = null;
        actionState = ActionState.returnToRing;
    }

    void Attack()
    {
        //This is to get the velocity from the target without calling the rigidbody.velocity, which always returns 0 because the movement isn't force based, but position based.
        if (targetFirstFrame == null)
        {
            targetFirstFrame = attackTarget.transform.position;
            return;
        }
        else if (targetSecondFrame == null)
        {
            targetSecondFrame = attackTarget.transform.position;
            targetMovement = targetSecondFrame - targetFirstFrame;
        }
        else
        {
            targetFirstFrame = targetSecondFrame;
            targetSecondFrame = attackTarget.transform.position;
            targetMovement = (targetSecondFrame - targetFirstFrame) / Time.fixedDeltaTime;
        }

        if ((transform.position - attackTarget.transform.position).sqrMagnitude < 7 * 7) //If the AI is within the right distance to start pushing
        {
            bool hasPredictedAngle = AnglePrediction(transform.position, 13f, attackTarget.transform.position, targetMovement, out Vector3 predictionDirection);

            if (baseAI.currObjective != AiObjective.push && hasPredictedAngle)
            {
                if (Vector3.Distance(transform.position, attackTarget.transform.position) < 3) //If they're close enough for prediction to not matter. This saves time for getting the prediction right and just push!
                {
                    baseAI.rotateToY = Quaternion.LookRotation(attackTarget.transform.position - transform.position).eulerAngles.y;
                    float angel = Vector3.Angle(transform.forward, attackTarget.transform.position);

                    if (Mathf.Abs(angel) < 40)
                    {
                        baseAI.currObjective = AiObjective.push;
                        StartCoroutine(ResetState(0.1f));
                    }
                    return;
                }

                baseAI.rotateToY = Quaternion.LookRotation(predictionDirection - transform.position).eulerAngles.y;
                float predictionAngle = Vector3.Angle(transform.forward, attackTarget.transform.position + predictionDirection);

                if (Mathf.Abs(predictionAngle) < 10) //If the AI is looking within certain fov of the predicted direction angle
                {
                    baseAI.currObjective = AiObjective.push;
                    StartCoroutine(ResetState(0.1f));
                }
            }
            else
            {
                baseAI.rotateToY = Quaternion.LookRotation(attackTarget.transform.position - transform.position).eulerAngles.y;
            }
        }
    }

    bool CalculatePriority(List<GameObject> powerupList)
    {
        //float threshold = 100f;
        //float initial = 70f;
        //float constant = 160f;

        List<GameObject> chosenPowerUps = new List<GameObject>();
        List<float> distanceValues = new List<float>();

        for (int i = 0; i < powerupList.Count; i++)
        {
            //float priority = initial + (constant / Vector3.Distance(transform.position, powerupList[i].transform.position));
            if (Vector3.Distance(transform.position, powerupList[i].transform.position) < powerUpSearchRadius)
            {
                chosenPowerUps.Add(powerupList[i]);
                distanceValues.Add(Vector3.Distance(transform.position, powerupList[i].transform.position));
            }
            //if (priority >= threshold) //If the powerup is close enough to be worth getting
            //{
            //    chosenPowerUps.Add(powerupList[i]);
            //    priorityValues.Add(Vector3.Distance(transform.position, powerupList[i].transform.position));
            //}
        }

        //Picks the closest powerup and sets it as it's target
        if (chosenPowerUps.Count > 1)
        {
            int bestChoiceIndex = 0;
            float lowestValue = 100f;
            int timesRunThrough = 0;

            foreach (var value in distanceValues)
            {
                if (value < lowestValue)
                {
                    bestChoiceIndex = timesRunThrough;
                    lowestValue = value;
                }

                timesRunThrough++;
            }

            powerUpTarget = chosenPowerUps[bestChoiceIndex];
            Debug.Log(lowestValue + ", " + powerUpTarget.name);
            return true;
        }
        else if (chosenPowerUps.Count == 1)
        {
            powerUpTarget = chosenPowerUps[0];
            return true;
        }
        else
        {
            return false;
        }
    }


    //List<float> what = new List<float>();

    void DodgeCalculation()
    {
        //For every sumo, update their current position and compare it to their previous position, and from that create a velocity value, of which you get the speed.
        for (int i = 0; i < otherSumos.Count; i++)
        {
            Vector3 direction = new Vector3(0, 0, 0);
            float speed = 0f;

            if (sumoFirstFrame.Count == 0)
            {
                for (int j = 0; j < otherSumos.Count; j++)
                {
                    sumoFirstFrame.Add(otherSumos[j].transform.position);
                }
                
                sumoFirstFrame[i] = otherSumos[i].transform.position;
                return;
            }
            else if (sumoSecondFrame.Count == 0)
            {
                for (int j = 0; j < otherSumos.Count; j++)
                {
                    sumoSecondFrame.Add(otherSumos[j].transform.position);
                }
            }

            //This is where it sadly starts to get iffy. The position of the sumo between 2 frames can sometimes be exactly the same, resulting in a speed of 0.
            //However, if we take multiple frames, then we can get a speed value. Through Debugging, the push speed is averaged at 13f per 5 frames.
            //The average speed for 10 frames, is 22f, so the check will use 18 for now.
            //This is to deal with the values that are a bit off. Using more frames gives more accurate results, but results in a longer reaction time.
            else
            {
                if(frameCounter == 0)
                {
                    sumoFirstFrame[i] = sumoSecondFrame[i];
                }
                else if(frameCounter == 10)
                {
                    sumoSecondFrame[i] = otherSumos[i].transform.position;
                    direction = (sumoSecondFrame[i] - sumoFirstFrame[i]) / Time.fixedDeltaTime;
                    speed = direction.magnitude;

                //    what.Add(speed);

                //    float min = 1110f;
                //    float max = 0f;
                //    float sum = 0f;
                //    float average = 0f;
                //    for (int j = 0; j < what.Count; j++)
                //    {
                //        if(what[j] < min)
                //        {
                //            min = what[j];
                //        }
                //        if(what[j] > max)
                //        {
                //            max = what[j];
                //        }

                //        sum += what[j];
                //    }

                //    average = sum / what.Count;

                //    Debug.Log(min + ", " + max + ", " + average);
                //    Debug.Log(direction);
                //    Debug.Log(speed);
                }
            }
            Debug.DrawRay(otherSumos[i].transform.position, otherSumos[i].transform.forward, new Color(255, 0, 255));

            //If the speed of the other sumo is comparable to a push attack
            if (!hasDodged && speed > 18 && speed < 200) //Safety checks: The speed can sometimes be somewhere around 300 at the first calculation + 0.1 is added to make some wiggle room for error in the calculations
            {
                RaycastHit ray;

                if (Physics.Raycast(otherSumos[i].transform.position, otherSumos[i].transform.forward, out ray, 1000f))
                {
                    if (ray.transform.CompareTag("Player")) //If the other sumo is looking/aiming at the AI
                    {
                        if ((transform.position - otherSumos[i].transform.position).sqrMagnitude < 2 * 2) //If the AI is close (this makes sure that the AI doesn't randomly dodges due to calculation errors, and so they can make up for prediction angles)
                        {
                            actionState = ActionState.evade;
                            baseAI.currObjective = AiObjective.dodge;
                            hasDodged = true;
                            StartCoroutine(ResetDodge());
                        }
                    }
                }
            }
        }

        if(frameCounter == 10)
        {
            frameCounter = 0;
        }
        else
        {
            frameCounter++;
        }
    }

    bool AnglePrediction(Vector3 AIPosition, float pushSpeed, Vector3 targetPosition, Vector3 targetVelocity, out Vector3 desiredPushDirection)
    {
        float targetSpeed = targetVelocity.magnitude;
        Vector3 targetToAIDirection = AIPosition - targetPosition;
        float targetToAIDistance = targetToAIDirection.magnitude;
        targetToAIDirection.Normalize();
        float cosValue = Vector3.Dot(targetToAIDirection, targetVelocity.normalized);

        //https://en.wikipedia.org/wiki/Law_of_cosines
        //Using the Law of Cosines, we can figure out the correct angle to push using the time it takes to get there
        //c^2 = a^2 + b^2 - 2ab * cos(y)
        //A in this case is the distance between the AI and the target
        //B in this case is the distance between the target and the impact zone
        //C in this case is the distance between the AI and the impact zone

        /*
                  B
         Target ______ Impact Point
                \    /
              A  \  /  C
                  \/
                  AI
        */

        //We can build the equasion into:
        //c^2 = a^2 + b^2 - 2ab * cos(y)
        //b^2 = a^2 + b^2 - 2ab * cos(y)
        //a^2 - 2ab * cos(y) = 0
        //a - 2b * cos(y) = 0
        //a / (2 * cos(y)) = b
        //0.5 * A / cos(y)) = b
        //speed = distance / time --> distance = time * speed
        //time * targetSpeed = 0.5 * A / cos(y)

        //time = 0.5 * A / (targetSpeed / cos(y))

        float timeValue = 0f;

        if (Mathf.Approximately(pushSpeed * pushSpeed, targetSpeed * targetSpeed)) //If the speed of the AI and the target are about the same (This can only occur if the target is pushing)
        {
            if (cosValue > 0) //Safety check just in case (if the angle isn't backwards from the AI)
            {
                timeValue = 0.5f * targetToAIDistance / (targetSpeed * cosValue); //The time it would take to reach the impact point
            }
            else
            {
                desiredPushDirection = Vector3.zero;
                return false;
            }
        }
        else
        {
            //https://en.wikipedia.org/wiki/Quadratic_formula
            //this uses that quadratic equasion you might've learned in high school of which you never realized how you would need it :P 
            //Quadratic Equasion:
            //x = (-b +- √(b^2 - 4ac)) / 2a

            float a = (pushSpeed * pushSpeed) - (targetSpeed * targetSpeed);
            float b = 2f * targetToAIDistance * targetSpeed * cosValue;
            float c = -(targetToAIDistance * targetToAIDistance);

            //(b^2 - 4ac) - this part of the equasion is called the "Discriminant"
            float discrimininant = b * b - 4f * a * c;

            if (discrimininant < 0) //Another safety check! If the target is faster than the AI charging (this should only happen when someone cheats)
            {
                desiredPushDirection = Vector3.zero;
                return false;
            }
            else
            {
                //For neatne
                //time = (-b + - √(b ^ 2 - 4ac)) / 2a

                float timeValue1 = (-b + Mathf.Sqrt(discrimininant)) / (2 * a);
                float timeValue2 = (-b - Mathf.Sqrt(discrimininant)) / (2 * a);
                timeValue = Mathf.Min(timeValue1, timeValue2); //Get the lowest value for the earliest successful aim value

                if (timeValue < Mathf.Epsilon) //Yet another safety check! If the lowest value would make the AI push too early to actually hit
                {
                    timeValue = Mathf.Max(timeValue1, timeValue2);
                }

                if (timeValue < Mathf.Epsilon) //There were no correct aim solutions found
                {
                    desiredPushDirection = Vector3.zero;
                    return false;
                }
            }
        }

        desiredPushDirection = targetVelocity + (-targetToAIDirection / timeValue); //Returns the Vector3 of the desired push direction
        return true;

    }

    IEnumerator ResetDodge()
    {
        yield return new WaitForSeconds(0.2f);
        baseAI.currObjective = AiObjective.idle;
        hasDodged = false;
        actionState = ActionState.returnToRing;
        
    }

    IEnumerator ResetState(float time)
    {
        yield return new WaitForSeconds(time);
        baseAI.currObjective = AiObjective.idle;
    }
}
