using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSelect : MonoBehaviour
{
    // This method can be linked to the button's OnClick event in the Inspector.
    public void OnButtonClick()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
