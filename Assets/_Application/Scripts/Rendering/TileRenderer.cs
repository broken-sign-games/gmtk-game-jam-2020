using DG.Tweening;
using GMTK2020.Data;
using UnityEngine;

namespace GMTK2020.Rendering
{
    public class TileRenderer : MonoBehaviour
    {
        [SerializeField] private Transform vialTransform = null;
        [SerializeField] private SpriteRenderer glassSprite = null;
        [SerializeField] private SpriteRenderer liquidSprite = null;
        [SerializeField] private SpriteRenderer corkSprite = null;
        [SerializeField] private SpriteMask liquidMask = null;

        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private ParticleSystem bubbles = null;
        [SerializeField] private float tileFadeDuration = 0.25f;
        [SerializeField] private float corkDistance = 0.1f;
        [SerializeField] private float corkMoveDuration = 0.5f;
        [SerializeField] private float corkFadeDuration = 0.5f;
        [SerializeField] private float tiltFrequency = 1f;
        [SerializeField] private float tiltAmplitude = 10f;
        [SerializeField] private TileData tileData = null;

        private Tile tile;

        private void Update()
        {
            if (!tile.Marked)
                return;

            vialTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * tiltFrequency) * tiltAmplitude);
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;

            transform.localPosition = (Vector2)tile.Position;

            glassSprite.sprite = tileData.VialSpriteMap[tile.Color];
            corkSprite.sprite = tileData.CorkSpriteMap[tile.Color];
            liquidSprite.sprite = tileData.LiquidSpriteMap[tile.Color];
            liquidMask.sprite = tileData.LiquidSpriteMap[tile.Color];

            UpdatePrediction();
        }

        public void UpdatePrediction()
        {
            if (tile.Marked)
            {
                bubbles.Play();
                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.transform.DOLocalMoveY(corkDistance, corkMoveDuration));
                seq.Append(corkSprite.DOFade(0, corkFadeDuration));
            }
            else
            {
                bubbles.Stop();
                vialTransform.localRotation = Quaternion.identity;

                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.DOFade(1, corkFadeDuration));
                seq.Append(corkSprite.transform.DOLocalMoveY(0, corkMoveDuration));
            }
        }

        public Tween ShowCorrectPrediction()
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(glassSprite.DOFade(0.0f, tileFadeDuration));
            
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