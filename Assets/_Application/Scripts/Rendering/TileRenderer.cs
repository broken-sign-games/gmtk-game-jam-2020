using DG.Tweening;
using GMTK2020.Data;
using TMPro;
using UnityEngine;

namespace GMTK2020.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private SpriteMask liquidMask = null;
        [SerializeField] private ParticleSystem bubbles = null;
        [SerializeField] private float tileFadeDuration = 0.25f;
        [SerializeField] private float rotationDuration = 1f;
        [SerializeField] private TileData tileData = null;

        private Tile tile;
        private SpriteRenderer sprite;
        private Sequence shakingTween;
        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            UpdatePrediction();
            transform.localPosition = (Vector2)tile.Position;
            liquidMask.sprite = tileData.LiquidMaskMap[tile.Color];
        }

        public void UpdatePrediction()
        {
            sprite.sprite = tile.Marked
                ? tileData.MarkedSpriteMap[tile.Color]
                : tileData.UnmarkedSpriteMap[tile.Color];

            if (tile.Marked)
            {
                bubbles.Play();
                shakingTween = DOTween.Sequence();
                shakingTween.Append(transform.DOLocalRotate(new Vector3(0, 0, 5f), rotationDuration/2).SetEase(Ease.Linear));
                shakingTween.Append(transform.DOLocalRotate(new Vector3(0, 0, -5f), rotationDuration).SetEase(Ease.Linear).SetLoops(int.MaxValue, LoopType.Yoyo));
            }
            else
            {
                bubbles.Stop();
                if (shakingTween != null)
                {
                    shakingTween.Kill();
                    transform.DOLocalRotate(Vector3.zero, rotationDuration);
                    shakingTween = null;
                }
            }
        }

        public Tween ShowCorrectPrediction()
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(sprite.DOFade(0.0f, tileFadeDuration));
            
            return seq;
        }

        public void ShowIncorrectPrediction()
        {
            incorrectBackground.gameObject.SetActive(true);
        }

        public void ShowMissingPrediction()
        {
            missingPredictionIndicator.gameObject.SetActive(true);
        }
    }
}