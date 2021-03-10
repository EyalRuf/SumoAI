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

    public static int numberOfPlayerInRing;
    public static float pointModifier = 1;

    private void Update()
    {
        switch (numberOfPlayerInRing)
        {
            case 0:
                pointModifier = 3;
                break;
            case 1:
                pointModifier = 3;
                break;
            case 2:
                pointModifier = 2;
                break;
            case 3:
                pointModifier = 2;
                break;
            case 4:
                pointModifier = 1;
                break;
            case 5:
                pointModifier = 1;
                break;
            case 6:
                pointModifier = 1;
                break;
            default:
                pointModifier = 1;
                numberOfPlayerInRing = 0;
                break;
        }
    }

    public void WinCheck(GameObject winner)
    {
        winBox.SetActive(true);
        gameStopped = true;
        winBox.transform.GetChild(0).GetComponent<Text>().text = "Player " + winner.name + " has Won";
    }
}
