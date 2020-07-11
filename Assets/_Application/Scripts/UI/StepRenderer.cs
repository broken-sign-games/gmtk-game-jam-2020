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

        public Tween ShowSuccess()
        {
            return successIndicator.DOFade(1f, fadeDuration);
        }

        public Tween ShowFailure()
        {
            return failureIndicator.DOFade(1f, fadeDuration);
        }
    }
}