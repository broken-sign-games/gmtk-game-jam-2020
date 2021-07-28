using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class ChainSegment : MonoBehaviour
    {
        [SerializeField] private Image wideLink = null;
        [SerializeField] private Image flatLink = null;
        [SerializeField] private Image wideLinkTop = null;
        [SerializeField] private Image wideLinkBottom = null;
        [SerializeField] private Image wideLinkShadowed = null;

        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float snapDuration = 0.5f;

        public Tween AnimateAppearance(ChainSegment previousSegment)
        {
            Sequence seq = DOTween.Sequence();

            seq.Join(wideLinkTop.DOFade(1f, fadeInDuration));
            seq.Join(wideLinkBottom.DOFade(1f, fadeInDuration));

            var topRectTransform = wideLinkTop.GetComponent<RectTransform>();
            var bottomRectTransform = wideLinkBottom.GetComponent<RectTransform>();
            seq.Append(topRectTransform.DOAnchorPosY(0, snapDuration).SetEase(Ease.InCubic));
            seq.Join(bottomRectTransform.DOAnchorPosY(0, snapDuration).SetEase(Ease.InCubic));
            seq.Join(flatLink.DOFade(1f, snapDuration).SetEase(Ease.InCubic));
            if (previousSegment)
                seq.Join(previousSegment.AddShadow());

            seq.AppendCallback(() =>
            {
                wideLinkTop.DeactivateObject();
                wideLinkBottom.DeactivateObject();
                wideLink.ActivateObject();
            });

            return seq;
        }

        public Tween AddShadow()
        {
            Sequence seq = DOTween.Sequence();

            seq.Join(wideLinkShadowed.DOFade(1f, snapDuration).SetEase(Ease.InCubic));
            seq.AppendCallback(() => wideLink.DeactivateObject());

            return seq;
        }
    } 
}
