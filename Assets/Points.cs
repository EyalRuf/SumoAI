using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    int points;
    bool inCircle;
    Coroutine inside;
    [SerializeField] Text pointCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter(Collider other)
    {
        inside = StartCoroutine(PointCycle());
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(inside);
    }

    IEnumerator PointCycle()
    {
        yield return new WaitForSeconds(0.5f);
        points++;
        pointCount.text = points.ToString();
        inside = StartCoroutine("PointCycle");
    }
}

