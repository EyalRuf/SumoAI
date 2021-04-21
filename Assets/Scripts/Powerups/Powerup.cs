using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour
{
    // Perhaps this could've been done with a protected EventHandler where they can subscribe to. onSumoCollsion?.Invoke()
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
