using UnityEngine;

// Don't push test objects to your repo!!
public class BoxCast : MonoBehaviour
{
    float m_MaxDistance;
    float m_Speed;
    bool m_HitDetect;
    bool m_HitDetected;

    Collider m_Collider;
    RaycastHit m_Hit;
    RaycastHit m_Hiter;

    void Start()
    {
        //Choose the distance the Box can reach to
        m_MaxDistance = 1.5f;
        m_Speed = 5.0f;
        m_Collider = GetComponent<Collider>();
    }

    void Update()
    {
        //Simple movement in x and z axes
        float xAxis = Input.GetAxis("Horizontal") * m_Speed;
        float zAxis = Input.GetAxis("Vertical") * m_Speed;
        transform.Translate(new Vector3(xAxis, 0, zAxis));
    }

    void FixedUpdate()
    {
        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the GameObject's Collider(could also just use the GameObject's position), half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data

        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gos)
        {
            if (go.Equals(this.gameObject))
                continue;
            m_HitDetected = Physics.BoxCast(go.transform.position, new Vector3(go.transform.localScale.x * 2, go.transform.localScale.y, go.transform.localScale.z * 2), go.transform.forward, out m_Hiter, go.transform.rotation, m_MaxDistance);

        }

        //Debug.Log(m_Hit.point);
        m_HitDetect = Physics.BoxCast(m_Collider.bounds.center, new Vector3(transform.localScale.x * 2, transform.localScale.y, transform.localScale.z * 2), transform.forward, out m_Hit, transform.rotation, m_MaxDistance);
        if (m_HitDetect && m_Hit.transform.tag == "Player")
        {
            //Output the name of the Collider your Box hit
            //Debug.Log("Hit : " + m_Hit.collider.name);
        }
    }


    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    /*void OnDrawGizmos()
    {

        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in gos)
        {
            if (go.Equals(this.gameObject))
                continue;

            Gizmos.DrawRay(go.transform.position, go.transform.forward * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(go.transform.position + go.transform.forward * m_MaxDistance, new Vector3(go.transform.localScale.x*2, go.transform.localScale.y, go.transform.localScale.z*2));

            if (m_HitDetected && m_Hiter.transform.tag == "Player")
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(go.transform.position, go.transform.forward * m_MaxDistance);
                //Draw a cube at the maximum distance
                Gizmos.DrawWireCube(go.transform.position + go.transform.forward * m_MaxDistance, new Vector3(go.transform.localScale.x * 2, go.transform.localScale.y, go.transform.localScale.z * 2));
            }


        }        
        
        //Check if there has been a hit yet
        if (m_HitDetect && m_Hit.transform.tag == "Player")
        {
            Gizmos.color = Color.green;
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * m_MaxDistance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * m_MaxDistance, new Vector3(transform.localScale.x * 2, transform.localScale.y, transform.localScale.z * 2));
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            Gizmos.color = Color.white;
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * m_MaxDistance, new Vector3(transform.localScale.x * 2, transform.localScale.y, transform.localScale.z * 2));
        }
    }*/
}