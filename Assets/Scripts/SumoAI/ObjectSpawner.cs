using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    public GameHandler gameHandler;
    public LayerMask spawnLayerMask;

    [Header("Settings obstacles")]
    [SerializeField] List<GameObject> obstacles;
    [SerializeField] int numberOfObstacles;
    [Tooltip("Radius of the ring. Length from center to the side.")]
    [SerializeField] float radius;
    [Tooltip("This should be the position of the ring itself.")]
    [SerializeField] Vector3 offset;
    [Tooltip("The Y position offset of the ring, so it'll spawn inside it instead of at a random height.")]
    [SerializeField] float yPosition;

    [Header("Settings powerups")]
    [SerializeField] List<GameObject> powerUps;
    [SerializeField] float minSpawnIncrement;
    [SerializeField] float maxSpawnIncrement;
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
        SpawnObstacle(numberOfObstacles);
        StartCoroutine(SpawnPowerUp());
    }

    public void SpawnObstacle(int obstacleCount)
    {
        //Spawns a set number of obstacles at the start INSIDE the ring
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 spawnPoint = Random.insideUnitSphere * radius;
            spawnPoint += offset;
            spawnPoint.y = yPosition;
            Instantiate(obstacles[Random.Range(0, obstacles.Count)], spawnPoint, Quaternion.identity);
        }
    }

    IEnumerator SpawnPowerUp()
    {
        canSpawn = false;

        yield return new WaitForSeconds(Random.Range(minSpawnIncrement, maxSpawnIncrement));

        if (!gameHandler.gameStopped)
        {
            if (minSpawnIncrement < 0 || maxSpawnIncrement < 0 || minSpawnIncrement >= maxSpawnIncrement) //Safety check
            {
                Debug.LogError("No powerups will spawn, because the min and max time values are wrong.");
                yield break;
            }

            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), yPosition, Random.Range(minZ, maxZ));

            //Tries to spawn a powerup, if something is already blocking the way, then it retries until it can freely spawn
            while (!canSpawn)
            {
                preventSpawningArray = Physics.OverlapSphere(spawnPosition, collisionCheckRadius, spawnLayerMask);

                if (preventSpawningArray.Length == 0)
                {
                    Instantiate(powerUps[Random.Range(0, powerUps.Count)], spawnPosition, Quaternion.identity);
                    canSpawn = true;
                } else
                {
                    spawnPosition = new Vector3(Random.Range(minX, maxX), yPosition, Random.Range(minZ, maxZ));
                }
            }

            // See GameHandler for feedback
            yield return StartCoroutine(SpawnPowerUp());
        }
    }
}
