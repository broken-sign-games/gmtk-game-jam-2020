using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class StepRenderer : MonoBehaviour
    {
        [SerializeField] private Image successIndicator = null;
        [SerializeField] private Image failureIndicator = null;
        [SerializeField] private float fadeDuration = 0.5f;

        private SoundManager SoundManager = null;

        private void Start()
        {
            SoundManager = FindObjectOfType<SoundManager>();
        }

        public void ShowSuccess(int step)
        {
            SoundManager?.PlayEffect(SoundManager.Effect.STEP_CORRECT, step);
            successIndicator.gameObject.SetActive(true);
        }

        public void ShowFailure()
        {
            SoundManager?.PlayEffect(SoundManager.Effect.STEP_WRONG);
            failureIndicator.gameObject.SetActive(true);
        }
    }
}