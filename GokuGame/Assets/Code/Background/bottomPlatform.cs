using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bottomPlatform : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject tilePlatformPrefab; // Assign the prefab to be spawned.
    public float spawnInterval = 3f;        // Time (in seconds) between spawns.
    public float spawnY = -4.5f;            // Vertical position for spawning platforms.
    public float spawnMargin = 10f;         // How far off-screen to the right the platform spawns.

    [Header("Movement & Destruction Settings")]
    public float moveSpeed = 5f;            // Speed at which platforms move left.
    public float destroyMargin = 10f;       // Extra margin beyond the left edge of the camera for destruction.

    private float nextSpawnTime = 0f;
    private List<GameObject> spawnedPlatforms = new List<GameObject>();

    void Update()
    {
        // Spawn a new platform if it's time.
        if (Time.time >= nextSpawnTime)
        {
            SpawnTilePlatform();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // Update all spawned platforms (move them left and check for destruction).
        UpdatePlatforms();
    }

    void SpawnTilePlatform()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Main camera not found!");
            return;
        }
        
        // Determine the right edge of the camera view.
        Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
        // Set the spawn position to be off-screen to the right.
        Vector3 spawnPosition = new Vector3(rightEdge.x + spawnMargin, spawnY, 0);
        
        GameObject platform = Instantiate(tilePlatformPrefab, spawnPosition, Quaternion.identity);
        spawnedPlatforms.Add(platform);
    }

    void UpdatePlatforms()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;
        
        // Determine the left edge of the camera view.
        Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));

        // Loop backwards through the list for safe removal.
        for (int i = spawnedPlatforms.Count - 1; i >= 0; i--)
        {
            if (spawnedPlatforms[i] == null)
            {
                spawnedPlatforms.RemoveAt(i);
                continue;
            }
            
            // Move the platform left.
            spawnedPlatforms[i].transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            
            // If the platform's position is past the left edge (with added margin), destroy it.
            if (spawnedPlatforms[i].transform.position.x < leftEdge.x - destroyMargin)
            {
                Destroy(spawnedPlatforms[i]);
                spawnedPlatforms.RemoveAt(i);
            }
        }
    }
}
