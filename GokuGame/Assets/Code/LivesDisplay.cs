using UnityEngine;
using TMPro; // Use TextMeshPro

public class LivesDisplay : MonoBehaviour
{
    public Samuri player;
    private TextMeshProUGUI livesText;

    void Awake()
    {
        livesText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (player != null)
        {
            livesText.text = "Lives: " + player.lives;
        }
    }
}
