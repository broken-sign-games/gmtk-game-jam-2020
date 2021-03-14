using GMTK2020.Audio;
using GMTK2020.SceneManagement;
using GMTK2020.TutorialSystem;
using UnityEngine;

namespace GMTK2020.UI
{
    public class Exit : MonoBehaviour
    {
        public void StopOrQuit()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void GoToMainMenu()
        {
            TutorialManager.Instance.CompleteActiveTutorial();
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            SceneLoader.Instance.LoadScene(SceneID.Menu);
        }
    } 
}
