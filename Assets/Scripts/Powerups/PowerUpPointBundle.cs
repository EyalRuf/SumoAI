using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPointBundle : Powerup
{
    [SerializeField] int pointCount;
    AudioSource pickUpSound;

    private void Start()
    {
        pickUpSound = GetComponent<AudioSource>();
    }

    public int GetPointCount()
    {
        return pointCount;
    }

    protected override void OnSumoCollision(Sumo sumo)
    {
        base.OnSumoCollision(sumo);
        Points somuPoints = sumo.GetComponent<Points>();
        somuPoints.UpdatePoints(GetPointCount());
        pickUpSound.Play();
        Destroy(gameObject);
    }
}
