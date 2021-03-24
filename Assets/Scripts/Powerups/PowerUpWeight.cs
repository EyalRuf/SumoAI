using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpWeight : Powerup
{
    [Header("Settings")]
    [SerializeField] float weightGain;
    [SerializeField] float duration;
    AudioSource pickUpSound;

    private void Start()
    {
        pickUpSound = GetComponent<AudioSource>();
    }

    protected override void OnSumoCollision(Sumo sumo)
    {
        base.OnSumoCollision(sumo);
        Rigidbody SumoRB = sumo.GetComponent<Rigidbody>();
        SumoRB.mass += weightGain;
        transform.position = new Vector3(0, Random.Range(-300, -500), 0);
        pickUpSound.Play();
        StartCoroutine(ResetWeightValue(SumoRB));
    }

    IEnumerator ResetWeightValue(Rigidbody SumoRB)
    {
        yield return new WaitForSeconds(duration);
        SumoRB.mass -= weightGain;
        Destroy(gameObject);
    }
}
