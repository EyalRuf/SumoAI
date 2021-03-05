using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumoBaseAI : Sumo
{
    [Header("AI")]
    public AiObjective currObjective;
    public float rotateTo;

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
                // based on rotationDir decides 
                //float myAngle = transform.eulerAngles.y % 360; // 100
                //float destinationAngle = rotateTo % 360; // 200
                //float rotationDir = ((myAngle - destinationAngle) % 360) >= 180 ? -1 : 1;

                // if they're not really close
                //if (Mathf.Abs(myAngle - destinationAngle) > 0.1f)
                //{
                    Rotate(rotateTo);
                //}
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