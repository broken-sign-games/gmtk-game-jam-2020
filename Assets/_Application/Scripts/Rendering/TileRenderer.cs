﻿using DG.Tweening;
using GMTK2020.Audio;
using GMTK2020.Data;
using System;
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
        [SerializeField] private SpriteRenderer tileHighlight = null;
        [SerializeField] private SpriteMask liquidMask = null;

        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private ParticleSystem bubbles = null;
        [SerializeField] private ParticleSystem pop = null;
        [SerializeField] private ParticleSystem popRing = null;
        [SerializeField] private ParticleSystem puff = null;

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

        [SerializeField] private float matchShrinkDuration = 0.5f;

        [SerializeField] private float fallingSpeed = 0.75f;
        [SerializeField] private float staggeredFallingDelay = 0.1f;
        [SerializeField] private Ease fallingEase = Ease.OutBounce;

        [SerializeField] private float landingShakeDuration = 0.2f;
        [SerializeField] private Vector3 landingShakeStrength = new Vector3(1f, 1f, 0f);
        [SerializeField] private int landingShakeVibrato = 10;
        [SerializeField] private float landingShakeRandomness = 90f;

        [SerializeField] private TileData tileData = null;

        private Tile tile;

        private void Update()
        {
            if (!tile.Marked)
                return;

            vialTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * tiltFrequency) * tiltAmplitude);
        }

        public void SetTile(Tile tile)
            => SetTile(tile, tile.Position);

        public void SetTile(Tile tile, Vector2Int initialPosition)
        {
            this.tile = tile;

            transform.localPosition = (Vector2)initialPosition;

            glassSprite.sprite = tileData.VialSpriteMap[tile.Color];
            corkSprite.sprite = tileData.CorkSpriteMap[tile.Color];
            liquidSprite.sprite = tileData.LiquidSpriteMap[tile.Color];
            liquidMask.sprite = tileData.LiquidSpriteMap[tile.Color];
            glowSprite.sprite = tileData.GlowSpriteMap[tile.Color];
            glowSprite.color = tileData.GlowColor[tile.Color];

            Color highlightColor = tileData.PopDropletColor[tile.Color];
            highlightColor.a = 0;
            tileHighlight.color = highlightColor;

            ParticleSystem.MainModule mainPop = pop.main;
            mainPop.startColor = tileData.PopDropletColor[tile.Color];
            ParticleSystem.MainModule mainPopRing = popRing.main;
            mainPopRing.startColor = tileData.PopDropletColor[tile.Color];
            ParticleSystem.MainModule mainPuff = puff.main;
            mainPuff.startColor = tileData.PopDropletColor[tile.Color];
        }

        public Tween UpdatePrediction()
        {
            if (tile.Marked)
            {
                SoundManager.Instance.PlayEffect(SoundEffect.SelectTile);
                bubbles.Play();
                pop.Play();
                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.transform.DOLocalMoveY(corkDistance, corkMoveDuration));
                seq.Append(corkSprite.DOFade(0, corkFadeDuration));
                seq.Insert(0, vialTransform.DOPunchScale(Vector3.one * clickPulseScale, clickPulseDuration, 0, 0));

                Sequence glowSeq = DOTween.Sequence();
                glowSeq.Append(glowSprite.DOFade(glowFlashOpacity, glowFadeDuration));
                glowSeq.Join(tileHighlight.DOFade(glowFlashOpacity, glowFadeDuration));
                glowSeq.Append(glowSprite.DOFade(glowOpacity, glowFadeDuration));
                glowSeq.Join(tileHighlight.DOFade(glowOpacity, glowFadeDuration));

                seq.Insert(0, glowSeq);

                return seq;
            }
            else
            {
                SoundManager.Instance.PlayEffectWithRandomPitch(SoundEffect.DeselectTile);
                bubbles.Stop();
                vialTransform.localRotation = Quaternion.identity;

                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.DOFade(1, corkFadeDuration));
                seq.Append(corkSprite.transform.DOLocalMoveY(0, corkMoveDuration).SetEase(Ease.OutBack));
                seq.Insert(0, transform.DOPunchScale(Vector3.one * clickPulseScale, clickPulseDuration, 0, 0));
                seq.Insert(0, glowSprite.DOFade(0, glowFadeDuration));
                seq.Insert(0, tileHighlight.DOFade(0, glowFadeDuration));

                return seq;
            }
        }

        public Tween TransitionToInert()
        {
            bubbles.Stop();
            vialTransform.localRotation = Quaternion.identity;

            Sequence seq = DOTween.Sequence();

            seq.Join(glowSprite.DOFade(0, glowFadeDuration));
            seq.Join(tileHighlight.DOFade(0, glowFadeDuration));

            liquidSprite.enabled = false;

            return seq;
        }

        public Tween FallToCurrentPosition(Vector2Int from)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform
                .DOLocalMove((Vector3Int)tile.Position, Mathf.Sqrt((from.y - tile.Position.y) / fallingSpeed))
                .SetEase(fallingEase)
                .SetDelay(tile.Position.y * staggeredFallingDelay));

            seq.Append(vialTransform.DOShakePosition(landingShakeDuration, landingShakeStrength * (from.y - tile.Position.y) / 9, landingShakeVibrato, landingShakeRandomness));

            return seq;
        }

        public Tween MatchAndDestroy()
        {
            puff.Play();
            Sequence seq = DOTween.Sequence();

            seq.Append(vialTransform.DOScale(0, matchShrinkDuration).SetEase(Ease.OutBack));
            seq.Join(tileHighlight.DOFade(0, matchShrinkDuration));
            seq.AppendCallback(() => Destroy(gameObject));

            return seq;
        }

        public Tween Destroy()
        {
            puff.Play();
            Sequence seq = DOTween.Sequence();

            seq.Append(vialTransform.DOScale(0, matchShrinkDuration).SetEase(Ease.OutBack));
            seq.Join(corkSprite.transform.DOScale(0, matchShrinkDuration).SetEase(Ease.OutBack));
            seq.Join(tileHighlight.DOFade(0, matchShrinkDuration));
            seq.AppendCallback(() => Destroy(gameObject));

            return seq;
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