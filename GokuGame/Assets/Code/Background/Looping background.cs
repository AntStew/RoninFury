using UnityEngine;

public class Loopingbackground : MonoBehaviour
{
    // Speed at which the background moves
    public float speed = 2.0f;
    // X position at which the background will reset to the right side
    public float leftBoundary = -23.0f;
    // X starting position when resetting to the right side
    public float rightStart = 0f;

    void Update()
    {
        // Move the background to the left continuously
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // When the background goes beyond the left boundary, reset its position to the right
        if (transform.position.x <= leftBoundary)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = rightStart;
            transform.position = newPosition;
        }
    }
}
