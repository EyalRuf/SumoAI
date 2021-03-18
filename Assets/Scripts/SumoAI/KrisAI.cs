using UnityEngine;
using System.Collections;

public class KrisAI : SumoBaseAI
{
    // Save info about other sumos
    public SumoBaseAI otherSumo;
    public Vector3 ArenaCenter = new Vector3(0,0,0);

    // Update is called once per frame
    void Update()
    {
        // get other sumo pos & angle towards
        Vector2 dest = new Vector2(otherSumo.transform.position.x, otherSumo.transform.position.z);
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
                
        Debug.Log(Vector2.Distance(dest, currentPos));

        if (Vector2.Distance(dest,currentPos)<10)
        {
            Quaternion rotationTowards = Quaternion.LookRotation(otherSumo.transform.position - transform.position);
            float angleTowards = rotationTowards.eulerAngles.y;
            this.destination = dest;
            this.rotateToY = angleTowards;
            currObjective = AiObjective.idle;
            if (Vector2.Distance(dest, currentPos) < 5)
            {
                currObjective = AiObjective.push;
            }
        }
        else if(Vector2.Distance(dest, currentPos) <1 && !usedPush)
        {
            Quaternion rotationTowards = Quaternion.LookRotation(ArenaCenter - transform.position);
            float angleTowards = rotationTowards.eulerAngles.y;
            this.destination = ArenaCenter;
            this.rotateToY = angleTowards;
            currObjective = AiObjective.dodge;
        }
        else
        {
            Quaternion rotationTowards = Quaternion.LookRotation(ArenaCenter - transform.position);
            float angleTowards = rotationTowards.eulerAngles.y;
            this.destination = ArenaCenter;
            this.rotateToY = angleTowards;
            currObjective = AiObjective.idle;
        }




    }
}
