using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumoBaseAI : Sumo
{
    [Header("AI")]
    public AiObjective currObjective;
    public Vector2 destination;
    public float rotateToY;

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        PerformObjective();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            Sumo other = collision.collider.GetComponent<Sumo>();

            if (isPushing)
            {
                other.ActionLockPush(pushDuration / 2);
            } else if (other.isPushing)
            {
                ActionLockPush(other.pushDuration / 2);
            }
        }

        if(currObjective == AiObjective.push)
        {
            aud.soundEmitter.clip = aud.impact;
            aud.soundEmitter.Play();
        } 
    }

    void PerformObjective ()
    {
        if (!actionLocked)
        {
            MoveTowards(new Vector3(destination.x, 0, destination.y));
            RotateTo(rotateToY);

            if (currObjective == AiObjective.push && !isPushing)
            {
                Push();
            }
            else if (currObjective == AiObjective.dodge && !isDodging)
            {
                Dodge();
            }
            else if (currObjective == AiObjective.idle)
            {
                // Do nothing :D
            }
        }
    }
}