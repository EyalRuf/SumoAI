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

    enum ActionState { evade, dodge, returnToRing, attack, gettingPowerup};
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

    void Start()
    {
        baseAI = GetComponent<SumoBaseAI>();
        
        otherSumos.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        otherSumos.Remove(gameObject);

        ring = GameObject.FindGameObjectWithTag("Ring"); //--TODO: Change this to a tag instead-//
        ringRadius = ring.transform.localScale.z / 2; //Get the radius of the ring
        GetInsideRingRadius = ringRadius * 0.7f; //Make the radius smaller to pursuade the AI to get deeper into the ring
    }

    void Update()
    {
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
                    if (potentialPowerUp.gameObject.CompareTag("PowerUp"))
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


        //Check if the player is inside the ring or not
        //More efficient way of checking for distance | (pointA - pointB).sqrMagnitude < distance * distance
        //if ((transform.position - ring.transform.position).sqrMagnitude < GetInsideRingRadius * GetInsideRingRadius) //If the player is inside the ring closer to the center
        //{
        //    ringState = RingState.closeInside;

        //    if (actionState != ActionState.gettingPowerup)
        //        DecideInsideRingAction();
        //}

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
            case ActionState.dodge: //--TODO: Make the player move unpredictably--//
                baseAI.destination = new Vector2(ring.transform.position.x, ring.transform.position.z);
                baseAI.rotateToY = Quaternion.LookRotation(ring.transform.position - transform.position).eulerAngles.y;
                DodgeCalculation();
                break;
            case ActionState.attack:
                baseAI.destination = new Vector2(attackTarget.transform.position.x, attackTarget.transform.position.z);

                //This is to get the velocity from the target without calling the rigidbody.velocity, which always returns 0 because the movement isn't force based, but position based.
                if (targetFirstFrame == null)
                {
                    targetFirstFrame = attackTarget.transform.position;
                    break;
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
                    //bool doom = PredictiveAim(transform.position, 13f, attackTarget.transform.position, movement, 0f, out Vector3 pushVelocity);
                    //bool doom = CalculateAngle(attackTarget.transform.position, transform.position, movement, 13f, out Vector3 pushVelocity);
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
                            break;
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

        if (playersInRing.Count > 0 && actionState != ActionState.evade) //If there are other players in the ring
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

    //void DecideOuterRingAction() //--TODO: Create check to make sure that the AI still attacks, but doesn't try to go back to the center--//
    //{
    //    actionState = ActionState.returnToRing;
    //}

    void DecideOutsideRingAction()
    {
        attackTarget = null;
        actionState = ActionState.returnToRing;
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

                direction = (sumoSecondFrame[i] - sumoFirstFrame[i]) / Time.fixedDeltaTime;
                speed = direction.magnitude;
            }
            else
            {
                direction = (sumoSecondFrame[i] - sumoFirstFrame[i]) / Time.fixedDeltaTime;
                speed = direction.magnitude;
            }

            //If the speed of the other sumo is comparable to a push attack
            if (!hasDodged && speed > baseAI.movementSpeed + 0.1f && speed < 20) //Safety checks: The speed can sometimes be somewhere around 300 at the first calculation + 0.1 is added to make some wiggle room for error in the calculations
            {
                RaycastHit ray;

                Debug.DrawRay(otherSumos[i].transform.position, otherSumos[i].transform.TransformDirection(transform.forward));
                if (Physics.Raycast(otherSumos[i].transform.position, otherSumos[i].transform.forward, out ray, 1000f))
                {
                    if (ray.transform.CompareTag("Player")) //If the other sumo is looking/aiming at the AI
                    {
                        if ((transform.position - otherSumos[i].transform.position).sqrMagnitude < 2 * 2) //If the AI is close (this makes sure that the AI doesn't randomly dodges due to calculation errors, and so they can make up for prediction angles)
                        {
                            actionState = ActionState.evade;
                            Debug.Log("Dodge!");
                            baseAI.currObjective = AiObjective.dodge;
                            hasDodged = true;
                        }
                    }
                }
            }
        }
    }

    bool AnglePrediction(Vector3 AIPosition, float pushSpeed, Vector3 targetPosition, Vector3 targetVelocity, out Vector3 desiredPushDirection)
    {
        float targetSpeed = targetVelocity.magnitude;
        Vector3 targetToAIDirection = AIPosition - targetPosition;
        float targetToAIDistance = targetToAIDirection.magnitude;
        targetToAIDirection.Normalize();
        float cosValue = Vector3.Dot(targetToAIDirection, targetVelocity.normalized);

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
        yield return new WaitForSeconds(baseAI.dodgeCD);
        hasDodged = false;
        actionState = ActionState.returnToRing;
    }

    IEnumerator ResetState(float time)
    {
        yield return new WaitForSeconds(time);
        baseAI.currObjective = AiObjective.idle;
    }

    //int PredictionCalculation(float a, float b, float c, out float root1, out float root2)
    //{
    //    //https://en.wikipedia.org/wiki/Quadratic_formula


    //    //ax2 + bx + c = 0
    //    //x = (-b +- √(b^2 - 4ac)) / 2a

    //    //To make the calculation stuff easier, I've represented the variable names as the corresponding equasion values

    //    //(b^2 - 4ac) - this part of the equasion is called the "Discriminant"
    //    float discriminant = b * b - 4 * a * c;

    //    if (discriminant < 0)
    //    {
    //        root1 = 0;
    //        root2 = 0;
    //        return 0; //No way to calculate the correct angle
    //    }

    //    //These are the two equasions, with the + and - values seperated into two variables
    //    root1 = (-b + Mathf.Sqrt(discriminant) / (2 * a));
    //    root2 = (-b - Mathf.Sqrt(discriminant) / (2 * a));

    //    if (discriminant > 0)
    //    {
    //        return 2; //If there are more than one possible angle
    //    }
    //    else
    //    {
    //        return 1; //If there's only one possible angle
    //    }
    //}

    //bool CalculateAngle(Vector3 targetPosition, Vector3 playerPosition, Vector3 targetVelocity, float pushSpeed, out Vector3 pushAngle)
    //{
    //    Vector2 direction = new Vector2(playerPosition.x, playerPosition.z) - new Vector2(targetPosition.x, targetPosition.z);
    //    float distance = direction.magnitude;

    //    //Visualize this as the angle when drawing a line from the player to the target, and the direction in which the target is moving
    //    float angle = Vector2.Angle(direction, targetVelocity) * Mathf.Deg2Rad;
    //    float targetSpeed = targetVelocity.magnitude;
    //    float speedDiff = targetSpeed / pushSpeed;

    //    if (PredictionCalculation(1 - speedDiff * speedDiff, 2 * speedDiff * distance * Mathf.Cos(angle), -(distance * distance), out float root1, out float root2) == 0)
    //    {
    //        //If there are no valid angles to push from
    //        pushAngle = Vector3.zero;
    //        return false;
    //    }
    //    else
    //    {
    //        float pushDirection = Mathf.Max(root1, root2);
    //        float timeToGetToTarget = pushDirection / pushSpeed;
    //        var interceptionPosition = targetPosition + targetVelocity * timeToGetToTarget;
    //        pushAngle = (interceptionPosition - playerPosition).normalized;
    //        return true;
    //    }
    //}

    ////////////////////////////////////////////////////////////////////////////////
    ////returns true if a valid solution is possible
    ////projectileVelocity will be a non-normalized vector representing the muzzle velocity of a lobbed projectile in 3D space
    ////if it returns false, projectileVelocity will be filled with a reasonable-looking attempt
    ////The reason we return true/false here instead of Vector3 is because you might want your AI to hold that shot until a solution exists
    ////This is meant to hit a target moving at constant velocity
    ////Full derivation by Kain Shin exists here:
    ////http://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php
    ////gravity is assumed to be a positive number. It will be calculated in the downward direction, feel free to change that if you game takes place in Spaaaaaaaace
    //static public bool PredictiveAim(Vector3 muzzlePosition, float projectileSpeed, Vector3 targetPosition, Vector3 targetVelocity, float gravity, out Vector3 projectileVelocity)
    //{
    //    Debug.Assert(projectileSpeed > 0, "What are you doing shooting at something with a projectile that doesn't move?");
    //    if (muzzlePosition == targetPosition)
    //    {
    //        //Why dost thou hate thyself so?
    //        //Do something smart here. I dunno... whatever.
    //        projectileVelocity = projectileSpeed * (Random.rotation * Vector3.forward);
    //        return true;
    //    }

    //    //Much of this is geared towards reducing floating point precision errors
    //    float projectileSpeedSq = projectileSpeed * projectileSpeed;
    //    float targetSpeedSq = targetVelocity.sqrMagnitude; //doing this instead of self-multiply for maximum accuracy
    //    float targetSpeed = Mathf.Sqrt(targetSpeedSq);
    //    Vector3 targetToMuzzle = muzzlePosition - targetPosition;
    //    float targetToMuzzleDistSq = targetToMuzzle.sqrMagnitude; //doing this instead of self-multiply for maximum accuracy
    //    float targetToMuzzleDist = Mathf.Sqrt(targetToMuzzleDistSq);
    //    Vector3 targetToMuzzleDir = targetToMuzzle;
    //    targetToMuzzleDir.Normalize();

    //    //Law of Cosines: A*A + B*B - 2*A*B*cos(theta) = C*C
    //    //A is distance from muzzle to target (known value: targetToMuzzleDist)
    //    //B is distance traveled by target until impact (targetSpeed * t)
    //    //C is distance traveled by projectile until impact (projectileSpeed * t)
    //    float cosTheta = (targetSpeedSq > 0)
    //        ? Vector3.Dot(targetToMuzzleDir, targetVelocity.normalized)
    //        : 1.0f;

    //    bool validSolutionFound = true;
    //    float t;
    //    if (Mathf.Approximately(projectileSpeedSq, targetSpeedSq))
    //    {
    //        //a = projectileSpeedSq - targetSpeedSq = 0
    //        //We want to avoid div/0 that can result from target and projectile traveling at the same speed
    //        //We know that C and B are the same length because the target and projectile will travel the same distance to impact
    //        //Law of Cosines: A*A + B*B - 2*A*B*cos(theta) = C*C
    //        //Law of Cosines: A*A + B*B - 2*A*B*cos(theta) = B*B
    //        //Law of Cosines: A*A - 2*A*B*cos(theta) = 0
    //        //Law of Cosines: A*A = 2*A*B*cos(theta)
    //        //Law of Cosines: A = 2*B*cos(theta)
    //        //Law of Cosines: A/(2*cos(theta)) = B
    //        //Law of Cosines: 0.5f*A/cos(theta) = B
    //        //Law of Cosines: 0.5f * targetToMuzzleDist / cos(theta) = targetSpeed * t
    //        //We know that cos(theta) of zero or less means there is no solution, since that would mean B goes backwards or leads to div/0 (infinity)
    //        if (cosTheta > 0)
    //        {
    //            t = 0.5f * targetToMuzzleDist / (targetSpeed * cosTheta);
    //        }
    //        else
    //        {
    //            validSolutionFound = false;
    //            t = PredictiveAimWildGuessAtImpactTime();
    //        }
    //    }
    //    else
    //    {
    //        //Quadratic formula: Note that lower case 'a' is a completely different derived variable from capital 'A' used in Law of Cosines (sorry):
    //        //t = [ -b � Sqrt( b*b - 4*a*c ) ] / (2*a)
    //        float a = projectileSpeedSq - targetSpeedSq;
    //        float b = 2.0f * targetToMuzzleDist * targetSpeed * cosTheta;
    //        float c = -targetToMuzzleDistSq;
    //        float discriminant = b * b - 4.0f * a * c;

    //        if (discriminant < 0)
    //        {
    //            //Square root of a negative number is an imaginary number (NaN)
    //            //Special thanks to Rupert Key (Twitter: @Arakade) for exposing NaN values that occur when target speed is faster than or equal to projectile speed
    //            validSolutionFound = false;
    //            t = PredictiveAimWildGuessAtImpactTime();
    //        }
    //        else
    //        {
    //            //a will never be zero because we protect against that with "if (Mathf.Approximately(projectileSpeedSq, targetSpeedSq))" above
    //            float uglyNumber = Mathf.Sqrt(discriminant);
    //            float t0 = 0.5f * (-b + uglyNumber) / a;
    //            float t1 = 0.5f * (-b - uglyNumber) / a;
    //            //Assign the lowest positive time to t to aim at the earliest hit
    //            t = Mathf.Min(t0, t1);
    //            if (t < Mathf.Epsilon)
    //            {
    //                t = Mathf.Max(t0, t1);
    //            }

    //            if (t < Mathf.Epsilon)
    //            {
    //                //Time can't flow backwards when it comes to aiming.
    //                //No real solution was found, take a wild shot at the target's future location
    //                validSolutionFound = false;
    //                t = PredictiveAimWildGuessAtImpactTime();
    //            }
    //        }
    //    }

    //    //Vb = Vt - 0.5*Ab*t + [(Pti - Pbi) / t]
    //    projectileVelocity = targetVelocity + (-targetToMuzzle / t);
    //    if (!validSolutionFound)
    //    {
    //        //PredictiveAimWildGuessAtImpactTime gives you a t that will not result in impact
    //        // Which means that all that math that assumes projectileSpeed is enough to impact at time t breaks down
    //        // In this case, we simply want the direction to shoot to make sure we
    //        // don't break the gameplay rules of the cannon's capabilities aside from gravity compensation
    //        projectileVelocity = projectileSpeed * projectileVelocity.normalized;
    //    }

    //    //FOR CHECKING ONLY (valid only if gravity is 0)...
    //    //float calculatedprojectilespeed = projectileVelocity.magnitude;
    //    //bool projectilespeedmatchesexpectations = (projectileSpeed == calculatedprojectilespeed);
    //    //...FOR CHECKING ONLY

    //    return validSolutionFound;
    //}
}
