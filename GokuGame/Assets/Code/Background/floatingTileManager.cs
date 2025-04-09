using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingTileManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject tilePrefab;         // The tile prefab to spawn.
    public float spawnMargin = 5f;          // How far off-screen to the right the tile spawns.
    public float minY = -2f;                // Minimum y position for spawn.
    public float maxY = 2f;                 // Maximum y position for spawn.
    
    [Header("Spawn Timing Settings")]
    public float minSpawnInterval = 0.5f;   // Minimum time (in seconds) between spawns.
    public float maxSpawnInterval = 2f;     // Maximum time (in seconds) between spawns.

    [Header("Tile Scale Settings")]
    public float minXScale = 2f;            // Minimum x scale.
    public float maxXScale = 5f;            // Maximum x scale.

    [Header("Movement & Destruction Settings")]
    public float moveSpeed = 5f;            // Speed at which the tile moves left.
    public float destroyMargin = 5f;        // How far off-screen to the left before destruction.

    private float nextSpawnTime = 0f;
    private List<GameObject> spawnedTiles = new List<GameObject>();

    void Update()
    {
        // Spawn a new tile if the randomized interval has passed.
        if (Time.time >= nextSpawnTime)
        {
            SpawnTile();
            // Randomize the next spawn interval between the set minimum and maximum.
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        // Move spawned tiles left and destroy them if they leave the camera view.
        UpdateTiles();
    }

    void SpawnTile()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Main Camera not found!");
            return;
        }

        // Get the right edge of the camera's view.
        Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
        // Choose a random y position between minY and maxY.
        float randomY = Random.Range(minY, maxY);
        // Calculate the spawn position.
        Vector3 spawnPos = new Vector3(rightEdge.x + spawnMargin, randomY, 0f);

        // Instantiate the tile prefab.
        GameObject tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);

        // Set a random x scale (keeping y and z scale unchanged).
        Vector3 newScale = tile.transform.localScale;
        newScale.x = Random.Range(minXScale, maxXScale);
        tile.transform.localScale = newScale;

        // Add the spawned tile to our list for movement & destruction.
        spawnedTiles.Add(tile);
    }

    void UpdateTiles()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        // Get the left edge of the camera's view.
        Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));

        // Loop backwards over the list for safe removal.
        for (int i = spawnedTiles.Count - 1; i >= 0; i--)
        {
            if (spawnedTiles[i] == null)
            {
                spawnedTiles.RemoveAt(i);
                continue;
            }

            // Move the tile to the left.
            spawnedTiles[i].transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            // If the tile's x position is off-screen on the left, destroy it.
            if (spawnedTiles[i].transform.position.x < leftEdge.x - destroyMargin)
            {
                Destroy(spawnedTiles[i]);
                spawnedTiles.RemoveAt(i);
            }
        }
    }
}
