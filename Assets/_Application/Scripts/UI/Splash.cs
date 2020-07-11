using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    private bool playerIsReady = false;

    void Update()
    {
        if (!playerIsReady && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            playerIsReady = true;
            SceneManager.LoadScene("TutorialScene");
        }
    }
}
