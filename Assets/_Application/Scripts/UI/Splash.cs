using GMTK2020.Audio;
using GMTK2020.SceneManagement;
using UnityEngine;

namespace GMTK2020.UI
{
    public class Splash : MonoBehaviour
    {
        private bool playerIsReady = false;

        private SoundManager soundManager;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();
        }

        private void Update()
        {
            if (!playerIsReady && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
            {
                playerIsReady = true;
                if (soundManager)
                    soundManager.PlayEffect(SoundManager.Effect.CLICK);
                LoadTutorialScene();
            }
        }

        private void LoadTutorialScene()
        {
            SceneLoader.Instance.LoadTutorialScene();
        }
    } 
}
