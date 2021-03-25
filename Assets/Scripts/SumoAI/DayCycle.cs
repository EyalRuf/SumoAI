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

    public GameObject spotlight1;
    public GameObject spotlight2;
    public GameObject spotlight3;
    public GameObject spotlight4;
    public GameObject spotlight5;
    public GameObject spotlight6;
    public GameObject spotlight7;
    public GameObject spotlight8;
    public GameObject spotlight9;
    public GameObject spotlight10;
    public GameObject spotlight11;
    public GameObject spotlight12;
    public GameObject spotlight13;


    private void Start()
    {
        light = GetComponent<Light>();
        StartCoroutine(cycle());
    }

    private void Update()
    {
        if(light.intensity <= 0)
        {
            spotlight1.SetActive(true);
            spotlight2.SetActive(true);
            spotlight3.SetActive(true);
            spotlight4.SetActive(true);
            spotlight5.SetActive(true);
            spotlight6.SetActive(true);
            spotlight7.SetActive(true);
            spotlight8.SetActive(true);
            spotlight9.SetActive(true);
            spotlight10.SetActive(true);
            spotlight11.SetActive(true);
            spotlight12.SetActive(true);
            spotlight13.SetActive(true);

        } else
        {
            spotlight1.SetActive(false);
            spotlight2.SetActive(false);
            spotlight3.SetActive(false);
            spotlight4.SetActive(false);
            spotlight5.SetActive(false);
            spotlight6.SetActive(false);
            spotlight7.SetActive(false);
            spotlight8.SetActive(false);
            spotlight9.SetActive(false);
            spotlight10.SetActive(false);
            spotlight11.SetActive(false);
            spotlight12.SetActive(false);
            spotlight13.SetActive(false);
        }
    }

    public IEnumerator cycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(cycleSpeed);
            rotate(); 
        }
    }
    public void rotate()
    {
        transform.Rotate(Vector3.up * rotateSpeed);
        xRotation = transform.rotation.eulerAngles.x;
        light.intensity = (xRotation > 120) ? 0 : intensity;
    }
}
