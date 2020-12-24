using GMTK2020.SceneManagement;
using UnityEngine;

namespace GMTK2020.UI
{
    public class Exit : MonoBehaviour
    {
        public void StopOrQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void GoToMainMenu()
        {
            SceneLoader.Instance.LoadMainMenuScene();
        }
    } 
}
