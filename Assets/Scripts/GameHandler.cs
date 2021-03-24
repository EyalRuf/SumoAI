using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int pointWinRequirement;
    [SerializeField] float cycleTimeLength;

    [Header("Value is for 1 AI in the ring, then drops by 1 per additionaly AI")]
    [SerializeField] int pointModifier;

    public bool gameStopped { get; private set; }
    [Space(20)]

    [SerializeField] GameObject winBox;

    [SerializeField] Transform ringTransform;
    [SerializeField] LayerMask playerLayerMask;

    private void Start()
    {
        if(pointModifier <= 0)
        {
            Debug.LogError("Point modifier is set to zero or a minus value. It will be set to 3 for now. Please assign it in the inspector accordingly");
            pointModifier = 3;
        }

        StartCoroutine(PointCycle());
    }

    IEnumerator PointCycle()
    {
        yield return new WaitForSeconds(cycleTimeLength);

        if(cycleTimeLength <= 0)
        {
            Debug.LogError("Cycle time is equal to zero or a minus value, please change this value accordingly. Points will not be given until this is fixed.");
            yield break;
        }

        if (!gameStopped)
        {
            Collider[] colliders;
            List<GameObject> playersInRing = new List<GameObject>();
            List<GameObject> winners = new List<GameObject>();
            int resetModifier = pointModifier;
            pointModifier += 1;

            colliders = Physics.OverlapSphere(ringTransform.position, 7f, playerLayerMask);

            foreach (var playerCol in colliders)
            {
                playersInRing.Add(playerCol.gameObject);
            }

            foreach (var player in playersInRing)
            {
                pointModifier--;
                if (pointModifier < 1)
                    pointModifier = 1;
            }


            List<Sumo> players = new List<Sumo>(FindObjectsOfType<Sumo>());
            foreach (var player in players)
            {
                player.GetComponent<Points>().UpdatePoints(pointModifier);

                if (player.GetComponent<Points>().points >= pointWinRequirement)
                {
                    winners.Add(player.gameObject);
                }
            }

            WinCheck(winners);

            pointModifier = resetModifier;
        }

        StartCoroutine("PointCycle");
    }

    public int ReturnRequirement()
    {
        return pointWinRequirement;
    }

    public void WinCheck(List<GameObject> winners)
    {
        if(winners.Count == 1)
        {
            winBox.SetActive(true);
            gameStopped = true;
            winBox.transform.GetChild(0).GetComponent<Text>().text = "Player " + winners[0].name + " has Won";
        }
        else if(winners.Count > 1)
        {
            winBox.SetActive(true);
            gameStopped = true;
            winBox.transform.GetChild(0).GetComponent<Text>().text = "DRAW!";
        }
    }
}
