using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPointBundle : MonoBehaviour
{
    [SerializeField] int pointCount;

    public int ReturnPointCount()
    {
        return pointCount;
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other.tag == "Player")
        {
            Points somuPoints = other.GetComponent<Points>();
            somuPoints.UpdatePoints(ReturnPointCount());
            Destroy(gameObject);
        }
    }
}
