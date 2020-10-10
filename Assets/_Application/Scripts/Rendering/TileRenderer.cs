using DG.Tweening;
using GMTK2020.Data;
using UnityEngine;

namespace GMTK2020.Rendering
{
    public class TileRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer vialSprite = null;
        [SerializeField] private SpriteRenderer liquidSprite = null;
        [SerializeField] private SpriteRenderer corkSprite = null;
        [SerializeField] private SpriteMask liquidMask = null;

        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private ParticleSystem bubbles = null;
        [SerializeField] private float tileFadeDuration = 0.25f;
        [SerializeField] private float corkDistance = 0.1f;
        [SerializeField] private float corkSpeed = 0.5f;
        [SerializeField] private float tiltFrequency = 1f;
        [SerializeField] private float tiltAmplitude = 10f;
        [SerializeField] private TileData tileData = null;

        private Tile tile;

        private void Update()
        {
            if (!tile.Marked)
                return;

            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * tiltFrequency) * tiltAmplitude);
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            UpdatePrediction();
            transform.localPosition = (Vector2)tile.Position;

            vialSprite.sprite = tileData.VialSpriteMap[tile.Color];
            corkSprite.sprite = tileData.CorkSpriteMap[tile.Color];
            liquidSprite.sprite = tileData.LiquidSpriteMap[tile.Color];
            liquidMask.sprite = tileData.LiquidSpriteMap[tile.Color];
        }

        public void UpdatePrediction()
        {
            if (tile.Marked)
            {
                bubbles.Play();
                corkSprite.transform.DOLocalMoveY(corkDistance, corkSpeed).SetSpeedBased().OnComplete(() => corkSprite.enabled = false);
            }
            else
            {
                bubbles.Stop();
                transform.localRotation = Quaternion.identity;
                corkSprite.enabled = true;
                corkSprite.transform.DOLocalMoveY(0, corkSpeed).SetSpeedBased();
            }
        }

        public Tween ShowCorrectPrediction()
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(vialSprite.DOFade(0.0f, tileFadeDuration));
            
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