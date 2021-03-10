using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpForce : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float forceGain;
    [SerializeField] float duration;
    float originalForce;

    private void OnTriggerEnter(Collider other)
    {
        originalForce = other.GetComponent<Sumo>().pushForce;
        other.GetComponent<Sumo>().pushForce += forceGain;
        transform.position = new Vector3(0, Random.Range(-300, -500), 0);
        StartCoroutine(ResetForceValue(other.gameObject));
    }

    IEnumerator ResetForceValue(GameObject affectedAI)
    {
        yield return new WaitForSeconds(duration);
        affectedAI.GetComponent<Sumo>().pushForce += originalForce;
        Destroy(gameObject);
    }
}
