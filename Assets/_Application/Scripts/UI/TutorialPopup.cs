using GMTK2020.Data;
using System;
using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class TutorialPopup : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tutorialText = null;
        [SerializeField] TutorialClip tutorialClip = null;

        public event Action Dismissed;

        public void ShowTutorial(Tutorial tutorial)
        {
            tutorialText.text = tutorial.Message;
            if (tutorial.ClipFrames.Length > 0)
            {
                tutorialClip.Frames = tutorial.ClipFrames;
                tutorialClip.StartAnimation();
                tutorialClip.gameObject.SetActive(true);
            }
            else
            {
                tutorialClip.gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
        }

        public void Dismiss()
        {
            gameObject.SetActive(false);
            Dismissed?.Invoke();
        }
    }
}