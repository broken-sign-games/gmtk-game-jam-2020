using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace GMTK2020.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text = null;
        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private SpriteRenderer stoneTile = null;
        [SerializeField] private Color incorrectColor = Color.red;
        [SerializeField] private Color correctColor = Color.green;
        [SerializeField] private float tileFadeDuration = 0.25f;
        [SerializeField] private float textFadeDuration = 0.25f;
        [SerializeField] private float textRiseDuration = 0.5f;
        [SerializeField] private float textRiseDistance = 0.5f;

        private SpriteRenderer sprite;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
        }


        public void UpdatePrediction(bool isPredicted)
        {
            text.text = isPredicted ? "+" : "";
        }

        public Tween ShowCorrectPrediction()
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(sprite.DOFade(0.0f, tileFadeDuration));
            seq.Join(text.DOColor(correctColor, 0.25f));
            seq.Join(text.transform.DOLocalMove(new Vector3(0f, textRiseDistance, 0f), textRiseDuration)
                .SetRelative()
                .SetEase(Ease.Linear));
            seq.Insert(tileFadeDuration, text.DOFade(0f, textFadeDuration));

            return seq;
        }

        public void ShowIncorrectPrediction()
        {
            incorrectBackground.gameObject.SetActive(true);
            text.color = incorrectColor;
        }

        public void ShowMissingPrediction()
        {
            missingPredictionIndicator.gameObject.SetActive(true);
        }

        public Tween Petrify()
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(sprite.DOFade(0f, tileFadeDuration));
            seq.Join(stoneTile.DOFade(1f, tileFadeDuration));
            seq.Join(text.DOFade(0f, tileFadeDuration));

            return seq;
        }

        public Tween Explode()
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(sprite.DOFade(0.0f, tileFadeDuration));
            seq.Join(stoneTile.DOFade(0.0f, tileFadeDuration));

            return seq;
        }
    }
}