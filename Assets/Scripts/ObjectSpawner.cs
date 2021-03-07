using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Settings obstacles")]
    [SerializeField] GameObject obstacle;
    [SerializeField] int numberOfObstacles;
    [Tooltip("Radius of the ring. Length from center to the side.")]
    [SerializeField] float radius;
    [Tooltip("This should be the position of the ring itself.")]
    [SerializeField] Vector3 offset;
    [Tooltip("The Y position offset of the ring, so it'll spawn inside it instead of at a random height.")]
    [SerializeField] float yPosition;

    [Header("Settings powerups")]
    [SerializeField] List<GameObject> powerUps;
    [SerializeField] float collisionCheckRadius;
    [SerializeField] float minX;
    [SerializeField] float maxX;
    [SerializeField] float minZ;
    [SerializeField] float maxZ;

    Collider[] preventSpawningArray;
    bool canSpawn;

    // Start is called before the first frame update
    void Start()
    {
        //Spawns a set number of obstacles at the start INSIDE the ring
        for (int i = 0; i < numberOfObstacles; i++)
        {
            Vector3 spawnPoint = Random.insideUnitSphere * radius;
            spawnPoint += offset;
            spawnPoint.y = yPosition;
            Instantiate(obstacle, spawnPoint, Quaternion.identity);
        }

        SpawnPowerUp();
    }

    public void SpawnPowerUp()
    {
        //Tries to spawn a powerup, if something is already blocking the way, then it retries until it can freely spawn
        while (!canSpawn)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), yPosition, Random.Range(minZ, maxZ));
            preventSpawningArray = Physics.OverlapSphere(spawnPosition, collisionCheckRadius, 9);

            if(preventSpawningArray.Length == 0)
            {
                Instantiate(powerUps[Random.Range(0, powerUps.Count)], spawnPosition, Quaternion.identity);
                canSpawn = true;
            }
        }
    }
}
