using GMTK2020;
using GMTK2020.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    [SerializeField] private LoadingPopup loadingPopup = null;

    private bool playerIsReady = false;

    private SoundManager SoundManager = null;

    private void Start()
    {
        SoundManager = FindObjectOfType<SoundManager>();
        TutorialSystem.ResetTutorial();
    }

    private void Update()
    {
        if (!playerIsReady && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            loadingPopup.Show();
            playerIsReady = true;
            SoundManager?.PlayEffect(SoundManager.Effect.CLICK);
            SceneManager.LoadScene("Match3Scene");
        }
    }
}
