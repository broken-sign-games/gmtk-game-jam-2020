using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class LevelExiter : MonoBehaviour
    {
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadNextLevel()
        {
            Debug.Log("Loading next level...");
        }
    }
}