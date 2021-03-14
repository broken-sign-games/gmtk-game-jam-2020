using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System.Threading.Tasks;
using UnityEngine;

namespace GMTK2020.UI
{
    public class PlaybackButton : MonoBehaviour
    {
        [SerializeField] private Playback playback = null;
        [SerializeField] private Canvas maskableCanvas = null;
        [SerializeField] private Canvas unmaskableCanvas = null;

        private TutorialManager tutorialManager;

        private void Awake()
        {
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
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            tutorialManager.CompleteActiveTutorial();
            playback.StartReaction();
        }

        private Task OnTutorialReady(Tutorial tutorial)
        {
            if (tutorial.PlaybackButtonAvailable)
                transform.SetParent(unmaskableCanvas.transform);

            return Task.CompletedTask;
        }

        private Task OnTutorialCompleted(Tutorial tutorial)
        {
            transform.SetParent(maskableCanvas.transform);

            return Task.CompletedTask;
        }
    } 
}
