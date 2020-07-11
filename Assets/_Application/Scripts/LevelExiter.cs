using GMTK2020.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class LevelExiter : MonoBehaviour
    {
        public void RestartLevel()
        {
            LoadLevelScene();
        }

        public void LoadNextLevel()
        {
            ++GameProgression.CurrentLevelIndex;
            // TODO: Load "CONGRATIONS YOU DONE IT" screen when current level exceed level sequence
            LoadLevelScene();
        }

        private static void LoadLevelScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}