using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpForce : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float forceGain;
    [SerializeField] float duration;

    void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (other.tag == "Player")
        {
            Sumo somu = other.GetComponent<Sumo>();
            somu.pushForce += forceGain;
            transform.position = new Vector3(0, Random.Range(-300, -500), 0);
            StartCoroutine(ResetForceValue(somu));
        }
    }

    IEnumerator ResetForceValue(Sumo somu)
    {
        yield return new WaitForSeconds(duration);
        somu.pushForce -= forceGain;
        Destroy(gameObject);
    }
}
