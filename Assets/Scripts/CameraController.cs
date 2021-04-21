using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public GameHandler GH;
    public List<Camera> cameras;
    public List<Transform> nametags;

    [Header("CameraController")]
    public bool isAutimatic;
    public bool autoSwitchRunning;
    public int currCameraIndex;
    public float minTimeBeweenCameraChange;
    public float maxTimeBeweenCameraChange;

    // Update is called once per frame
    void Update()
    {
        if (!GH.gameStopped)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                isAutimatic = !isAutimatic;
            }

            if (isAutimatic)
            {
                if (!autoSwitchRunning)
                    StartCoroutine(RotateBetweenAllCameras());
            } 

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ActivateCamera(mod(currCameraIndex + 1, cameras.Count));
            } else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                    
                ActivateCamera(mod(currCameraIndex - 1, cameras.Count));
            }
        }

        foreach (Transform nametag in nametags)
        {
            nametag.LookAt(cameras[currCameraIndex].transform);
        }
    }

    void ActivateCamera(int index)
    {
        Camera toActivate = cameras[index];
        Camera toDectivate = cameras[currCameraIndex];

        toActivate.gameObject.SetActive(true);
        toDectivate.gameObject.SetActive(false);

        currCameraIndex = index;
    }

    IEnumerator RotateBetweenAllCameras()
    {
        autoSwitchRunning = true;
        float timeToNextSwitch = Random.Range(minTimeBeweenCameraChange, maxTimeBeweenCameraChange);

        yield return new WaitForSeconds(timeToNextSwitch);
        autoSwitchRunning = false;

        if (isAutimatic)
        {
            ActivateCamera(mod(currCameraIndex + 1, cameras.Count));
            StartCoroutine(RotateBetweenAllCameras());
        }
    }

    int mod(int a, int n)
    {
        // Talk about being hard to read with all the different operators in there (including the ternary check)
        return (((a %= n) < 0) && n > 0) || (a > 0 && n < 0) ? a + n : a;
    }
}