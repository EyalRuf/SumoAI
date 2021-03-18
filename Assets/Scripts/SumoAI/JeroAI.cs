using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeroAI : SumoBaseAI
{
    private SumoBaseAI[] enemySumos;
    private GameObject ring;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        this.currObjective = AiObjective.idle;
        enemySumos = FindObjectsOfType<SumoBaseAI>();
        ring = GameObject.Find("ObjectiveCircle");
    }

    // Update is called once per frame
    void Update()
    {
        this.destination = new Vector2(ring.transform.position.x, ring.transform.position.z);
        this.rotateToY = Quaternion.LookRotation(ring.transform.position - transform.position).eulerAngles.y;

        foreach (SumoBaseAI enemySumo in enemySumos)
        {
            float range = Vector2.Distance(transform.position, enemySumo.transform.position);

            Debug.Log(range);

            if (range <= 1 && range > 0)
            {
                this.currObjective = AiObjective.dodge;
            }
            else
            {
                this.currObjective = AiObjective.idle;
            }
        }
    }
}
