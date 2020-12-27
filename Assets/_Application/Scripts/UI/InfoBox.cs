using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class InfoBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI infoText = null;
        [SerializeField] Button dismissTutorialButton = null;

        private TutorialManager tutorialManager;

        private void Start()
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

        public void DismissTutorial()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            tutorialManager.CompleteActiveTutorial();
        }

        private void OnTutorialReady(Tutorial tutorial)
        {
            infoText.text = tutorial.Message;
            if (tutorial.ShowDismissButton)
                dismissTutorialButton.ActivateObject();
        }

        private void OnTutorialCompleted(Tutorial tutorial)
        {
            infoText.text = "";
            dismissTutorialButton.DeactivateObject();
        }
    } 
}
