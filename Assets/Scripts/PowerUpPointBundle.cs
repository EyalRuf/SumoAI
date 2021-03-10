using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPointBundle : MonoBehaviour
{
    [SerializeField] int pointCount;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Points>().points += pointCount;
        other.GetComponent<Points>().UpdateUI();
        Destroy(gameObject);
    }
}
