using UnityEngine;
using System.Collections;

public class KrisAI : SumoBaseAI
{
    public GameObject FindClosestSumo()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");


        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            if (go.Equals(this.gameObject))
                continue;
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    //Find a sumo that's the closest to all sumos - center of the group
    public GameObject FindGroupedSumo()
    {
        GameObject[] gos1;
        gos1 = GameObject.FindGameObjectsWithTag("Player");

        GameObject grouped = null;
        float groupDistance = Mathf.Infinity;

        foreach (GameObject go1 in gos1)
        {
            if (go1.Equals(this.gameObject))
                continue;
            GameObject[] gos2;
            gos2 = GameObject.FindGameObjectsWithTag("Player");
            float sumDistance = 0;
            foreach(GameObject go2 in gos2)
            {
                Vector3 diff = go2.transform.position - go1.transform.position;
                float curDistance = diff.sqrMagnitude;
                sumDistance += curDistance;
            }
            if (groupDistance > sumDistance)
            {
                groupDistance = sumDistance;
                grouped = go1;
            }
        }
        return grouped;
    }

    // Update is called once per frame
    void Update()
    {
        if(FindClosestSumo() == FindGroupedSumo() && !isPushing)
        {
            currObjective = AiObjective.push;
            Debug.Log("dodged group");
            // get grouped sumo pos & angle towards
            Vector2 dest = new Vector2(FindClosestSumo().transform.position.x, FindClosestSumo().transform.position.z);
            Quaternion rotationAway = Quaternion.Inverse(FindClosestSumo().transform.rotation);
            float angleAway = rotationAway.eulerAngles.y;

            this.destination = dest;
            this.rotateToY = angleAway;
        }
        else if (!isDodging)
        {
            currObjective = AiObjective.dodge;
        }
        else
        {
            currObjective = AiObjective.idle;
        }
    }
}
