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
        [SerializeField] private SpriteRenderer glowSprite = null;
        [SerializeField] private SpriteMask liquidMask = null;

        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private ParticleSystem bubbles = null;
        [SerializeField] private ParticleSystem pop = null;

        [SerializeField] private float tileFadeDuration = 0.25f;

        [SerializeField] private float clickPulseScale = 1.1f;
        [SerializeField] private float clickPulseDuration = 0.5f;

        [SerializeField] private float corkDistance = 0.1f;
        [SerializeField] private float corkMoveDuration = 0.5f;
        [SerializeField] private float corkFadeDuration = 0.5f;

        [SerializeField] private float glowOpacity = 0.15f;
        [SerializeField] private float glowFlashOpacity = 0.4f;
        [SerializeField] private float glowFadeDuration = 0.1f;

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
            glowSprite.sprite = tileData.GlowSpriteMap[tile.Color];
            glowSprite.color = tileData.GlowColor[tile.Color];

            ParticleSystem.MainModule mainModule = pop.main;
            mainModule.startColor = tileData.PopDropletColor[tile.Color];
        }

        public void UpdatePrediction()
        {
            if (tile.Marked)
            {
                bubbles.Play();
                pop.Play();
                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.transform.DOLocalMoveY(corkDistance, corkMoveDuration));
                seq.Append(corkSprite.DOFade(0, corkFadeDuration));
                seq.Insert(0, transform.DOPunchScale(Vector3.one * clickPulseScale, clickPulseDuration, 0, 0));

                Sequence glowSeq = DOTween.Sequence();
                glowSeq.Append(glowSprite.DOFade(glowFlashOpacity, glowFadeDuration));
                glowSeq.Append(glowSprite.DOFade(glowOpacity, glowFadeDuration));

                seq.Insert(0, glowSeq);
            }
            else
            {
                bubbles.Stop();
                vialTransform.localRotation = Quaternion.identity;

                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.DOFade(1, corkFadeDuration));
                seq.Append(corkSprite.transform.DOLocalMoveY(0, corkMoveDuration).SetEase(Ease.OutBack));
                seq.Insert(0, transform.DOPunchScale(Vector3.one * clickPulseScale, clickPulseDuration, 0, 0));
                seq.Insert(0, glowSprite.DOFade(0, glowFadeDuration));
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