using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(Button))]
    public class ExitButton : MonoBehaviour
    {
        private TutorialManager tutorialManager;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            tutorialManager = TutorialManager.Instance;
            tutorialManager.TutorialReady += OnTutorialReady;
            tutorialManager.TutorialCompleted += OnTutorialCompleted;
        }

        private void OnDestroy()
        {
            tutorialManager.TutorialReady -= OnTutorialReady;
            tutorialManager.TutorialCompleted -= OnTutorialCompleted;
        }

        private Task OnTutorialReady(Tutorial tutorial)
        {
            button.enabled = false;

            return Task.CompletedTask;
        }

        private Task OnTutorialCompleted(Tutorial tutorial)
        {
            button.enabled = true;

            return Task.CompletedTask;
        }

        public void StopOrQuit()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    } 
}
