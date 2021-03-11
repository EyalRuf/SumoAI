using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [Header("Settings")]
    public int pointWinRequirement;
    [Space(20)]

    public bool gameStopped;
    [SerializeField] GameObject winBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void WinCheck(GameObject winner)
    {
        winBox.SetActive(true);
        gameStopped = true;
        winBox.transform.GetChild(0).GetComponent<Text>().text = "Player " + winner.name + " has Won";
    }
}
