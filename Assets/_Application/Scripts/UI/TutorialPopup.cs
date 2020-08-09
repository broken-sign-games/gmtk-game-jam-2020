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

        private SoundManager soundManager;

        private void Awake()
        {
            soundManager = FindObjectOfType<SoundManager>();
        }

        public void ShowTutorial(Tutorial tutorial)
        {
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.WIN);

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
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.CLICK);

            gameObject.SetActive(false);
            Dismissed?.Invoke();
        }
    }
}