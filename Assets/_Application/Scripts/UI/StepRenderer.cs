using GMTK2020.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class StepRenderer : MonoBehaviour
    {
        [SerializeField] private Image successIndicator = null;
        [SerializeField] private Image failureIndicator = null;
        
        private SoundManager soundManager;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();
        }

        public void ShowSuccess(int step)
        {
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.STEP_CORRECT, step);
            successIndicator.gameObject.SetActive(true);
        }

        public void ShowFailure()
        {
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.STEP_WRONG);
            failureIndicator.gameObject.SetActive(true);
        }
    }
}