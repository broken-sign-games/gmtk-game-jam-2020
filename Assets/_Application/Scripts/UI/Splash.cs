using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    private bool playerIsReady = false;

    void Update()
    {
        if (!playerIsReady && Input.anyKeyDown)
        {
            playerIsReady = true;
            SceneManager.LoadScene("LevelScene");
        }
    }
}
