using UnityEngine;
using UnityEngine.SceneManagement;

public class loadStartScreen : MonoBehaviour
{
    void Start()
    {
        // Wait for 16 seconds, then call LoadScene.
        Invoke("LoadScene", 16f);
    }

    void LoadScene()
    {
        // Loads the scene named "StartScreen".
        SceneManager.LoadScene("StartScreen");
    }
}
