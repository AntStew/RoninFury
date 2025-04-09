using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance for easy access.
    public static ScoreManager instance;

    public int score = 0;
    public TextMeshProUGUI scoreText; // Assign your TextMeshProUGUI element in the Inspector.

    void Awake()
    {
        // Setup the singleton instance.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
