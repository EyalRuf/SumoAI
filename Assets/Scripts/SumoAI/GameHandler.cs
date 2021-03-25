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
    [SerializeField] Text winText;

    [SerializeField] SphereCollider ringCollider;
    [SerializeField] LayerMask playerLayerMask;

    [SerializeField] AudioSource battleMusic;
    [SerializeField] AudioSource tenseMusic;
    [SerializeField] AudioSource applause;
    [SerializeField] AudioSource winTune;
    [SerializeField] AudioSource gong;

    bool tenseMusicPlaying;

    private void Start()
    {
        if(pointModifier <= 0)
        {
            Debug.LogError("Point modifier is set to zero or a minus value. It will be set to 3 for now. Please assign it in the inspector accordingly");
            pointModifier = 3;
        }

        battleMusic.Play();
        gong.Play();

        StartCoroutine(PointCycle());
    }

    void Update()
    {
        if (gameStopped)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Application.LoadLevel(0);
            }
        }
    }

    IEnumerator PointCycle()
    {
        pointModifier = 1;

        yield return new WaitForSeconds(cycleTimeLength);

        if(cycleTimeLength <= 0)
        {
            Debug.LogError("Cycle time is equal to zero or a minus value, please change this value accordingly. Points will not be given until this is fixed.");
            yield break;
        }

        if (!gameStopped)
        {
            List<Sumo> playersInRing = new List<Sumo>();
            List<GameObject> winners = new List<GameObject>();
            int resetModifier = pointModifier;
            pointModifier += 1;

            Collider[] playerColliders = Physics.OverlapSphere(ringCollider.bounds.center, ringCollider.radius * ringCollider.transform.localScale.y, playerLayerMask);
            foreach (var playerC in playerColliders)
            {
                playersInRing.Add(playerC.GetComponent<Sumo>());
            }

            foreach (var player in playersInRing)
            {
                pointModifier--;
                if (pointModifier < 1)
                    pointModifier = 1;
            }

            foreach (var player in playersInRing)
            {
                player.GetComponent<Points>().UpdatePoints(pointModifier);

                if (player.GetComponent<Points>().points >= pointWinRequirement * 0.9f && !tenseMusicPlaying)
                {
                    battleMusic.Stop();
                    tenseMusic.Play();
                    tenseMusicPlaying = true;
                }

                if (player.GetComponent<Points>().points >= pointWinRequirement)
                {
                    winners.Add(player.gameObject);
                }
            }

            WinCheck(winners);

            pointModifier = resetModifier;
        }

        StartCoroutine(PointCycle());
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
            winText.text = "Player " + winners[0].name + " has Won";
            tenseMusic.Stop();
            applause.Play();
            winTune.Play();
        }
        else if(winners.Count > 1)
        {
            winBox.SetActive(true);
            gameStopped = true;
            winText.text = "DRAW!";
            tenseMusic.Stop();
            applause.Play();
            winTune.Play();
        }
    }
}
