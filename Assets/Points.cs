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

    //References
    GameHandler gameHandler;

    // Start is called before the first frame update
    void Start()
    {
        gameHandler = GameObject.FindGameObjectWithTag("GameHandler").GetComponent<GameHandler>();
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

        if (!gameHandler.gameStopped)
        {
            points++;
            pointCount.text = points.ToString();

            if (points >= gameHandler.pointWinRequirement)
            {
                gameHandler.WinCheck(gameObject);
            }

            inside = StartCoroutine("PointCycle");
        }
        
    }
}

