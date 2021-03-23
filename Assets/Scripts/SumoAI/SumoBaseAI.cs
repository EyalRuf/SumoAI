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
            else if (currObjective == AiObjective.dodge && !usedDodge)
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