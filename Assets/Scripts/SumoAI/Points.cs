using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    //Variables
    public int points { get; private set; }
    [SerializeField] Text pointCount;

    //References
    GameHandler gameHandler;

    // Start is called before the first frame update
    void Start()
    {
        gameHandler = GameObject.FindGameObjectWithTag("GameHandler").GetComponent<GameHandler>();
    }

    public void UpdatePoints(int amount)
    {
        points += amount;
        pointCount.text = points.ToString();
    }
}

