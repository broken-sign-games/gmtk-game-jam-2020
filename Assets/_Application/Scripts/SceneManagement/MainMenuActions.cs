using UnityEngine;

namespace GMTK2020.SceneManagement
{
    public class MainMenuActions : MonoBehaviour
    {
        public void StartGame()
        {
            SceneLoader.Instance.LoadLevelScene();
        }

        public void GoToOptions()
        {
            SceneLoader.Instance.LoadOptionsScene();
        }

        public void GoToCredits()
        {
            SceneLoader.Instance.LoadCreditsScene();
        }
    } 
}
