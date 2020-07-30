using DG.Tweening;
using UnityEngine;

namespace GMTK2020.Rendering
{
    public class ClearedRowRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer indicator = null;
        [SerializeField] private SpriteRenderer laser = null;
        [SerializeField] private float indicatorFadeDuration = 0.5f;
        [SerializeField] private float laserFadeDuration = 0.25f;

        public Tween ShowIndicator()
        {
            return indicator.DOFade(1f, indicatorFadeDuration);
        }

        public Tween ClearRow()
        {
            return DOTween.Sequence()
                .Append(laser.DOFade(1f, laserFadeDuration))
                .Append(laser.DOFade(0f, laserFadeDuration))
                .Join(indicator.DOFade(0f, laserFadeDuration));
        }
    }

}