using UnityEngine;
using System.Collections;

public class EyalAI : SumoBaseAI
{
    // Save info about other sumos
    public SumoBaseAI otherSumo;

    // Update is called once per frame
    void Update()
    {
        // get other sumo pos & angle towards
        Vector2 dest = new Vector2(otherSumo.transform.position.x, otherSumo.transform.position.z);
        float angleTowards;
        //angleTowards = Vector2.Angle(transform.position, otherSumo.transform.position);
        Quaternion q = Quaternion.LookRotation(otherSumo.transform.position - transform.position);
        angleTowards = q.eulerAngles.y;

        this.destination = dest;
        this.rotateToY = angleTowards;
    }
}
