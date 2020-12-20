using GMTK2020.Data;
using GMTK2020.TutorialSystem;
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
            tutorialManager.CompleteActiveTutorial();
            playback.StartReaction();
        }

        private void OnTutorialReady(Tutorial tutorial)
        {
            if (tutorial.PlaybackButtonAvailable)
                transform.SetParent(unmaskableCanvas.transform);
        }

        private void OnTutorialCompleted(Tutorial tutorial)
        {
            transform.SetParent(maskableCanvas.transform);
        }
    } 
}
