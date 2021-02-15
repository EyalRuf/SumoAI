using System.Collections;
using UnityEngine;

/// <summary>
/// The vehicle that will do battle. This is the same for every participant in the arena.
/// Its 'brains' (the AI you'll write) will be assigned by the <seealso cref="CompetitionManager"/>
/// </summary>
public class PirateShipController : MonoBehaviour
{
    // the bullets and the locations on the prefab where they spawn from
    public GameObject CannonBallPrefab = null;
    public Transform CannonFrontSpawnPoint = null;
    public Transform CannonLeftSpawnPoint = null;
    public Transform CannonRightSpawnPoint = null;

    // the 'scanner' that allows the ship to 'see' its surroundings
    public GameObject Lookout = null;

    // sails can be used to indicate the state of the ship (attacking, fleeing, searching etc.)
    public GameObject[] sails = null;

    /// <summary>
    /// the AI that will control this ship. Is set by <seealso cref="CompetitionManager"/>.
    /// </summary>
    private BaseAI ai = null;

    // create a level playing field. Every ship has the same basic abilities
    private float BoatSpeed = 100.0f;
    private float SeaSize = 500.0f;
    private float RotationSpeed = 180.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Assigns the AI that steers this instance
    /// </summary>
    /// <param name="_ai"></param>
    public void SetAI(BaseAI _ai) {
        ai = _ai;
        ai.Ship = this;
    }

    /// <summary>
    /// Tell this ship to start battling
    /// Should be called only once
    /// </summary>
    public void StartBattle() {
        Debug.Log("test");
        StartCoroutine(ai.RunAI());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    /// <summary>
    /// If a ship is inside the 'scanner', its information (distance and name) will be sent to the AI
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay(Collider other) {
        if (other.tag == "Boat") {
            ScannedRobotEvent scannedRobotEvent = new ScannedRobotEvent();
            scannedRobotEvent.Distance = Vector3.Distance(transform.position, other.transform.position);
            scannedRobotEvent.Name = other.name;
            ai.OnScannedRobot(scannedRobotEvent);
        }
    }

    /// <summary>
    /// Move this ship ahead by the given distance
    /// </summary>
    /// <param name="distance">The distance to move</param>
    /// <returns></returns>
    public IEnumerator __Ahead(float distance) {
        int numFrames = (int)(distance / (BoatSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Translate(new Vector3(0f, 0f, BoatSpeed * Time.fixedDeltaTime), Space.Self);
            Vector3 clampedPosition = Vector3.Max(Vector3.Min(transform.position, new Vector3(SeaSize, 0, SeaSize)), new Vector3(-SeaSize, 0, -SeaSize));
            transform.position = clampedPosition;

            yield return new WaitForFixedUpdate();            
        }
    }

    /// <summary>
    /// Move the ship backwards by the given distance
    /// </summary>
    /// <param name="distance">The distance to move</param>
    /// <returns></returns>
    public IEnumerator __Back(float distance) {
        int numFrames = (int)(distance / (BoatSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Translate(new Vector3(0f, 0f, -BoatSpeed * Time.fixedDeltaTime), Space.Self);
            Vector3 clampedPosition = Vector3.Max(Vector3.Min(transform.position, new Vector3(SeaSize, 0, SeaSize)), new Vector3(-SeaSize, 0, -SeaSize));
            transform.position = clampedPosition;

            yield return new WaitForFixedUpdate();            
        }
    }

    /// <summary>
    /// Turns the ship left by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator __TurnLeft(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Rotate(0f, -RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();            
        }
    }

    /// <summary>
    /// Turns the ship right by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator __TurnRight(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            transform.Rotate(0f, RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();            
        }
    }

    /// <summary>
    /// Sit and hold still for one (fixed!) update
    /// </summary>
    /// <returns></returns>
    public IEnumerator __DoNothing() {
        yield return new WaitForFixedUpdate();
    }

    /// <summary>
    /// Fire from the forward pointing cannon
    /// </summary>
    /// <param name="power">???</param>
    /// <returns></returns>
    public IEnumerator __FireFront(float power) {
        GameObject newInstance = Instantiate(CannonBallPrefab, CannonFrontSpawnPoint.position, CannonFrontSpawnPoint.rotation);
        yield return new WaitForFixedUpdate();
    }

    /// <summary>
    /// Fire from the left pointing cannon
    /// </summary>
    /// <param name="power">???</param>
    /// <returns></returns>
    public IEnumerator __FireLeft(float power) {
        GameObject newInstance = Instantiate(CannonBallPrefab, CannonLeftSpawnPoint.position, CannonLeftSpawnPoint.rotation);
        yield return new WaitForFixedUpdate();
    }

    /// <summary>
    /// fire from the right pointing cannon
    /// </summary>
    /// <param name="power">???</param>
    /// <returns></returns>
    public IEnumerator __FireRight(float power) {
        GameObject newInstance = Instantiate(CannonBallPrefab, CannonRightSpawnPoint.position, CannonRightSpawnPoint.rotation);
        yield return new WaitForFixedUpdate();
    }

    /// <summary>
    /// Change the color of the sails (for vanity or visualising state)
    /// </summary>
    /// <param name="color"></param>
    public void __SetColor(Color color) {
        foreach (GameObject sail in sails) {
            sail.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    /// <summary>
    /// Turn the sensor to the left by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator __TurnLookoutLeft(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            Lookout.transform.Rotate(0f, -RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();            
        }
    }

    /// <summary>
    /// Turn the sensor to the right by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator __TurnLookoutRight(float angle) {
        int numFrames = (int)(angle / (RotationSpeed * Time.fixedDeltaTime));
        for (int f = 0; f < numFrames; f++) {
            Lookout.transform.Rotate(0f, RotationSpeed * Time.fixedDeltaTime, 0f);

            yield return new WaitForFixedUpdate();            
        }
    }
}
