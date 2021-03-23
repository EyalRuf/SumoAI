using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other.tag == "Player")
        {
            Sumo sumo = other.GetComponent<Sumo>();
            OnSumoCollision(sumo);
        }
    }

    protected virtual void OnSumoCollision(Sumo somu)
    {

    }
}
