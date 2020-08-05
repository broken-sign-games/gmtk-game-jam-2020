using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    private bool playerIsReady = false;

    private SoundManager SoundManager = null;

    private void Start()
    {
        SoundManager = FindObjectOfType<SoundManager>();
    }

    void Update()
    {
        if (!playerIsReady && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            playerIsReady = true;
            SoundManager?.PlayEffect(SoundManager.Effect.CLICK);
            SceneManager.LoadScene("Match3Scene");
        }
    }
}
