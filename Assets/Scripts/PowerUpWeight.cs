﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpWeight : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float weightGain;
    [SerializeField] float duration;
    float originalWeight;

    private void OnTriggerEnter(Collider other)
    {
        originalWeight = other.GetComponent<Rigidbody>().mass;
        other.GetComponent<Rigidbody>().mass += weightGain;
        transform.position = new Vector3(0, Random.Range(-300, -500), 0);
        StartCoroutine(ResetWeightValue(other.gameObject));
    }

    IEnumerator ResetWeightValue(GameObject affectedAI)
    {
        yield return new WaitForSeconds(duration);
        affectedAI.GetComponent<Rigidbody>().mass = originalWeight;
        Destroy(gameObject);
    }
}
