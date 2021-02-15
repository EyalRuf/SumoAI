using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the class / component that manages the arena.
/// It generates / Instantiates the AI game objects and lets the games begin!
/// </summary>
public class CompetitionManager : MonoBehaviour
{
    // the prefab for the participants in the battle
    public GameObject PirateShipPrefab = null;

    // The positions where the participants will be instantiated
    // set in the inspector by dragging 4 gameobjects in the slots of the array
    public Transform[] SpawnPoints = null;

    // the list that keeps track of all the participants
    private List<PirateShipController> pirateShips = new List<PirateShipController>();

    /// <summary>
    /// creates the 4 ships that will do battle
    /// 4 ship prefabs will be instantated and each will be assigned an AI derived from BaseAI
    /// </summary>
    void Start()
    {
        BaseAI[] aiArray = new BaseAI[] {
            new IljaAI(), 
            new PondAI(), 
            new PondAI(), 
            new PondAI()
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject pirateShip = Instantiate(PirateShipPrefab, SpawnPoints[i].position, SpawnPoints[i].rotation);
            PirateShipController pirateShipController = pirateShip.GetComponent<PirateShipController>();
            pirateShipController.SetAI(aiArray[i]);
            pirateShips.Add(pirateShipController);
        }
    }

    /// <summary>
    /// Start Battle obviously should be called only once.
    /// Otherwise the ships will run multiple coroutines that manage their AI
    /// <seealso cref="PirateShipController"/>
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            foreach (var pirateShip in pirateShips) {
                pirateShip.StartBattle();
            }
        }
    }
}
