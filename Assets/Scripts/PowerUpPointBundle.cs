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


}
