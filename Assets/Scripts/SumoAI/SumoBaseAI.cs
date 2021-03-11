using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumoBaseAI : Sumo
{
    [Header("AI")]
    public AiObjective currObjective;
    public float rotateToY;

    // Update is called once per frame
    void Update()
    {
        PerformObjective();
    }

    void PerformObjective ()
    {
        if (!actionLocked)
        {
            if (currObjective == AiObjective.push && !isPushing)
            {
                Push();
            }
            else if (currObjective == AiObjective.dodge && !isDodging)
            {
                Dodge();
            }
            else if (currObjective == AiObjective.rotate)
            {
                Rotate(rotateToY);
            }
            else if (currObjective == AiObjective.move)
            {
                Movement();
            }
            else if (currObjective == AiObjective.wait)
            {
                // Do nothing :D
            }
        }
    }
}