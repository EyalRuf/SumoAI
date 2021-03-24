using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpWeight : Powerup
{
    [Header("Settings")]
    [SerializeField] float weightGain;
    [SerializeField] float duration;


    protected override void OnSumoCollision(Sumo sumo)
    {
        base.OnSumoCollision(sumo);
        Rigidbody SumoRB = sumo.GetComponent<Rigidbody>();
        SumoRB.mass += weightGain;
        transform.position = new Vector3(0, Random.Range(-300, -500), 0);
        StartCoroutine(ResetWeightValue(SumoRB));
    }

    IEnumerator ResetWeightValue(Rigidbody SumoRB)
    {
        yield return new WaitForSeconds(duration);
        SumoRB.mass -= weightGain;
        Destroy(gameObject);
    }
}
