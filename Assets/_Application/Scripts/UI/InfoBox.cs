using GMTK2020.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class InfoBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI infoText;
        [SerializeField] Button dismissTutorialButton;

        private TutorialSystem tutorialSystem;

        private void Start()
        {
            tutorialSystem = TutorialSystem.Instance;

            tutorialSystem.TutorialReady += OnTutorialReady;
            tutorialSystem.TutorialCompleted += OnTutorialCompleted;
        }

        private void OnDestroy()
        {
            tutorialSystem.TutorialReady -= OnTutorialReady;
            tutorialSystem.TutorialCompleted -= OnTutorialCompleted;
        }

        public void DismissTutorial()
        {
            tutorialSystem.CompleteActiveTutorial();
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
