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

    SumoBaseAI baseAI;
    List<GameObject> otherSumos = new List<GameObject>();

    GameObject ring;
    float ringRadius;
    float GetInsideRingRadius;

    enum RingState { inside, closeInside, outside };
    RingState ringState;

    enum ActionState { evade, returnToRing, attack, gettingPowerup };
    ActionState actionState;

    GameObject powerUpTarget;
    GameObject attackTarget;

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
            case ActionState.attack: //--TODO: Choose a target and chase that target until you or the target is out of the ring--//
                baseAI.destination = new Vector2(attackTarget.transform.position.x, attackTarget.transform.position.z);
                baseAI.rotateToY = Quaternion.LookRotation(attackTarget.transform.position - transform.position).eulerAngles.y;
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
        var playerCheck = Physics.OverlapSphere(ring.transform.position, ringRadius); 
        List<GameObject> playersInRing = new List<GameObject>();
        bool targetStillInRing = false;

        for (int i = 0; i < playerCheck.Length; i++)
        {
            if(playerCheck[i].CompareTag("Player") && playerCheck[i].gameObject != gameObject)
            {
                playersInRing.Add(playerCheck[i].gameObject);

                if (playerCheck[i].gameObject == attackTarget) //Check if the target is still in the ring or not
                    targetStillInRing = true;
            }
        }

        if (!targetStillInRing)
        {
            attackTarget = null;
        }

        if (playersInRing.Count > 0) //If there are other players in the ring
        {
            if(attackTarget == null)
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
            if(Vector3.Distance(transform.position, powerupList[i].transform.position) < powerUpSearchRadius)
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
                if(value < lowestValue)
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
}
