using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(Button))]
    public class PlaybackButton : MonoBehaviour
    {
        [SerializeField] private Playback playback = null;
        [SerializeField] private Image mask = null;

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

        public void OnClick()
        {
            button.interactable = false;
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            tutorialManager.CompleteActiveTutorial();
            playback.StartReaction();
        }

        private Task OnTutorialReady(Tutorial tutorial)
        {
            if (tutorial.PlaybackButtonAvailable)
                mask.enabled = true;
            else
                button.enabled = false;

            return Task.CompletedTask;
        }

        private Task OnTutorialCompleted(Tutorial tutorial)
        {
            mask.enabled = false;
            button.enabled = true;

            return Task.CompletedTask;
        }
    } 
}
