using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    public float rotateSpeed;
    public float cycleSpeed;

    public float xRotation;
    public float intensity;

    public Light light;

    public List<GameObject> spotlights;


    private void Start()
    {
        light = GetComponent<Light>();
        StartCoroutine(Cycle());
    }

    private void Update()
    {
        foreach (GameObject spot in spotlights)
        {
            spot.SetActive(light.intensity <= 0);
        }
    }

    public IEnumerator Cycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(cycleSpeed);
            Rotate(); 
        }
    }
    public void Rotate()
    {
        transform.Rotate(Vector3.up * rotateSpeed);
        xRotation = transform.rotation.eulerAngles.x;
        light.intensity = (xRotation > 120) ? 0 : intensity;
    }
}
