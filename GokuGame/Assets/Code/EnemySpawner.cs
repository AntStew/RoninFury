using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public GameObject enemyPrefab; // Assign your enemy prefab here.

    [Header("Spawn Position Settings")]
    public float spawnY = 5f;      // Set the fixed y position where enemies spawn.
    public float minX = -10f;      // Minimum x position for spawn.
    public float maxX = 10f;       // Maximum x position for spawn.

    [Header("Spawn Timing Settings")]
    public float minSpawnInterval = 5f;  // Initial minimum delay between spawns.
    public float maxSpawnInterval = 10f; // Initial maximum delay between spawns.

    [Header("Spawn Rate Increase Settings")]
    public float spawnRateAcceleration = 0.1f; // How much to reduce intervals per second.
    public float minAllowedInterval = 0.5f;      // Lower bound for the spawn interval.

    void Start()
    {
        // Start the coroutine that continuously spawns enemies.
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Calculate how much time has passed since the level started.
            float elapsedTime = Time.timeSinceLevelLoad;
            
            // Adjust the spawn intervals over time by subtracting a value proportional to elapsed time.
            float currentMinInterval = Mathf.Max(minSpawnInterval - spawnRateAcceleration * elapsedTime, minAllowedInterval);
            float currentMaxInterval = Mathf.Max(maxSpawnInterval - spawnRateAcceleration * elapsedTime, minAllowedInterval);
            
            // Wait for a random time between the current intervals.
            float waitTime = Random.Range(currentMinInterval, currentMaxInterval);
            yield return new WaitForSeconds(waitTime);

            // Choose a random x position between minX and maxX.
            float spawnX = Random.Range(minX, maxX);
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

            // Instantiate the enemy prefab at the chosen position.
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
