using UnityEngine;

public class gtileMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Set a non-zero speed in the Inspector

    void Update()
    {
        // Move the platform to the left continuously
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}
