using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int pointWinRequirement;
    [Space(20)]

    public bool gameStopped;
    [SerializeField] GameObject winBox;

    int numberOfPlayerInRing;
    float pointModifier = 1;

    private void Update()
    {
        switch (numberOfPlayerInRing)
        {
            case 0:
            case 1:
                pointModifier = 3;
                break;
            case 2:
            case 3:
                pointModifier = 2;
                break;
            case 4:
            case 5:
            case 6:
                pointModifier = 1;
                break;
            default:
                pointModifier = 1;
                numberOfPlayerInRing = 0;
                break;
        }
    }

    public void Entered(bool enter)
    {
        if (enter)
        {
            numberOfPlayerInRing++;
        }
        else
        {
            numberOfPlayerInRing--;
        }
    }

    public int GetModifier()
    {
        return (int)pointModifier;
    }

    public int GetRequirementValue()
    {
        return pointWinRequirement;
    }

    public void WinCheck(GameObject winner)
    {
        winBox.SetActive(true);
        gameStopped = true;
        winBox.transform.GetChild(0).GetComponent<Text>().text = "Player " + winner.name + " has Won";
    }
}
