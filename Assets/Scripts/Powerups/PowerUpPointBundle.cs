using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPointBundle : Powerup
{
    [SerializeField] int pointCount;

    public int GetPointCount()
    {
        return pointCount;
    }

    protected override void OnSumoCollision(Sumo sumo)
    {
        base.OnSumoCollision(sumo);
        Points somuPoints = sumo.GetComponent<Points>();
        somuPoints.UpdatePoints(GetPointCount());
        Destroy(gameObject);
    }
}
