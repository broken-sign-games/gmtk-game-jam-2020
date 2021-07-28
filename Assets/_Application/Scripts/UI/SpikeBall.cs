using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class SpikeBall : MonoBehaviour
    {
        [SerializeField] private Image mask = null;
        [SerializeField] private Image[] shrapnels = null;
        [SerializeField] private float flyInDistance = 400;
        [SerializeField] private float flyInSpeed = 400;
        [SerializeField] private float shrapnelDistance = 200;
        [SerializeField] private float shrapnelSpeed = 200;
        [SerializeField] private float shrapnelRotation = 1080;
        [SerializeField] private float relativeFadeOutDelay = 0.75f;

        public void EnableMask()
        {
            mask.enabled = true;
        }

        public void DisableMask()
        {
            mask.enabled = false;
        }

        public Tween AnimateAppearance(float targetXPos)
        {
            Sequence seq = DOTween.Sequence();

            var rectTransform = GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector2(targetXPos + flyInDistance, 0);

            float duration = flyInDistance / flyInSpeed;
            seq.Join(rectTransform.DOAnchorPosX(targetXPos, duration));
            seq.Join(GetComponent<Image>().DOFade(1, duration));

            return seq;
        }

        public Tween AnimateDestruction()
        {
            Sequence seq = DOTween.Sequence();

            seq.AppendCallback(() => {
                GetComponent<Image>().enabled = false;
                GetComponent<RotateTransform>().enabled = false;
                foreach (Image shrapnel in shrapnels)
                    shrapnel.ActivateObject();
            });

            float duration = shrapnelDistance / shrapnelSpeed;

            foreach (Image shrapnel in shrapnels)
            {
                RectTransform rectTransform = shrapnel.GetComponent<RectTransform>();
                seq.Insert(0, rectTransform.DOAnchorPos(rectTransform.anchoredPosition.normalized * shrapnelDistance, duration).SetRelative().SetEase(Ease.Linear));
                seq.Insert(0, rectTransform.DOLocalRotate(Vector3.forward * shrapnelRotation, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
                seq.Insert(duration * relativeFadeOutDelay, shrapnel.DOFade(0, duration * (1 - relativeFadeOutDelay)));
            }

            seq.AppendCallback(() => Destroy(gameObject));

            return seq;
        }
    } 
}
