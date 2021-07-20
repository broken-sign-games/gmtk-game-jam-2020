using DG.Tweening;
using GMTK2020.Audio;
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
        [SerializeField] private SpriteRenderer tileHighlight = null;
        [SerializeField] private SpriteMask vialMask = null;
        [SerializeField] private SpriteMask liquidMask = null;
        [SerializeField] private SpriteMask rainbowRoot = null;
        [SerializeField] private SpriteRenderer rainbowSprite = null;

        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private ParticleSystem bubbles = null;
        [SerializeField] private ParticleSystem pop = null;
        [SerializeField] private ParticleSystem popRing = null;
        [SerializeField] private ParticleSystem puff = null;
        [SerializeField] private ParticleSystem liquidEvap = null;
        [SerializeField] private ParticleSystem neckEvap = null;
        [SerializeField] private ParticleSystem dust = null;
        [SerializeField] private ParticleSystem weakEvaporation = null;
        [SerializeField] private ParticleSystem strongEvaporation = null;

        [SerializeField] private AudioSource bubblingAudioSource = null;
        [SerializeField] private AudioSource fallingAudioSource = null;
        [SerializeField] private AudioSource evaporatingAudioSource = null;

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

        [SerializeField] private float rainbowScrollSpeed = 1f;

        [SerializeField] private TileData tileData = null;

        [SerializeField] private ParticleSystem.MinMaxGradient rainbowColors = default;

        private Tile tile;

        private float rainbowLength;

        private void Start()
        {
            // Assumes rainbow sprite is tiled to two periods
            rainbowLength = rainbowSprite.size.y / 2;
        }

        private void Update()
        {
            if (tile.Inert)
                return;

            if (tile.Marked)
                vialTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * tiltFrequency) * tiltAmplitude);

            if (tile.Wildcard)
            {
                float y = rainbowSprite.transform.localPosition.y;

                y += rainbowScrollSpeed * Time.deltaTime;

                if (y > rainbowLength / 2)
                    y -= rainbowLength;

                rainbowSprite.transform.localPosition = Vector3.up * y;
            }
        }

        public void SetTile(Tile tile)
            => SetTile(tile, tile.Position);

        public void SetTile(Tile tile, Vector2Int initialPosition)
        {
            this.tile = tile;

            transform.localPosition = (Vector2)initialPosition;

            vialMask.sprite = tileData.VialMaskMap[tile.Color];
            corkSprite.sprite = tileData.CorkSpriteMap[tile.Color];
            liquidSprite.sprite = tileData.LiquidSpriteMap[tile.Color];
            liquidMask.sprite = tileData.LiquidSpriteMap[tile.Color];
            glowSprite.sprite = tileData.GlowSpriteMap[tile.Color];
            glowSprite.color = tileData.GlowColor[tile.Color];
            rainbowRoot.sprite = tileData.LiquidSpriteMap[tile.Color]; ;

            UpdateCracks(false);

            Color highlightColor = tileData.PopDropletColor[tile.Color];
            highlightColor.a = 0;
            tileHighlight.color = highlightColor;
            
            SetParticleColors(tileData.PopDropletColor[tile.Color]);
        }

        private void SetParticleColors(ParticleSystem.MinMaxGradient colors)
        {
            ParticleSystem.MainModule mainPop = pop.main;
            mainPop.startColor = colors;
            ParticleSystem.MainModule mainPopRing = popRing.main;
            mainPopRing.startColor = colors;
            ParticleSystem.MainModule mainPuff = puff.main;
            mainPuff.startColor = colors;
            ParticleSystem.MainModule mainLiquidEvap = liquidEvap.main;
            mainLiquidEvap.startColor = colors;
            ParticleSystem.MainModule mainNeckEvap = neckEvap.main;
            mainNeckEvap.startColor = colors;

            ParticleSystem.MainModule mainWeakEvaporation = weakEvaporation.main;
            colors.FixAlpha(mainWeakEvaporation.startColor.color.a);
            mainWeakEvaporation.startColor = colors;

            ParticleSystem.MainModule mainStrongEvaporation = strongEvaporation.main;
            colors.FixAlpha(mainStrongEvaporation.startColor.color.a);
            mainStrongEvaporation.startColor = colors;
        }

        public Tween UpdateCracks(bool playSound = true)
        {
            Sequence seq = DOTween.Sequence();

            if (playSound)
                seq.AppendCallback(() => SoundManager.Instance.PlayEffect(SoundEffect.VialCracked));

            seq.AppendCallback(() =>
            {
                glassSprite.sprite = tileData.VialSpriteMap[tile.Color][tile.Cracks];
                
                switch (tile.Cracks)
                {
                case 1:
                    weakEvaporation.Play();
                    break;
                case 2:
                    weakEvaporation.Stop();
                    strongEvaporation.Play();
                    break;
                }
            });

            seq.Append(PulseVial());

            return seq;
        }

        public Tween UpdatePrediction()
        {
            if (tile.Marked)
            {
                SoundManager.Instance.PlayEffect(SoundEffect.VialOpened);
                SoundManager.Instance.StartPlayingLoopEffect(bubblingAudioSource, SoundEffect.VialBubbling);
                bubbles.Play();
                pop.Play();
                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.transform.DOLocalMoveY(corkDistance, corkMoveDuration));
                seq.Append(corkSprite.DOFade(0, corkFadeDuration));
                seq.Insert(0, PulseVial());

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
                SoundManager.Instance.PlayEffect(SoundEffect.VialClosed);
                SoundManager.Instance.StopEffect(bubblingAudioSource);
                bubbles.Stop();
                vialTransform.localRotation = Quaternion.identity;

                DOTween.Complete(corkSprite);
                Sequence seq = DOTween.Sequence().SetId(corkSprite);
                seq.Append(corkSprite.DOFade(1, corkFadeDuration));
                seq.Append(corkSprite.transform.DOLocalMoveY(0, corkMoveDuration).SetEase(Ease.OutBack));
                seq.Insert(0, PulseVial());
                seq.Insert(0, glowSprite.DOFade(0, glowFadeDuration));
                seq.Insert(0, tileHighlight.DOFade(0, glowFadeDuration));

                return seq;
            }
        }

        public Tween TransitionToInert()
        {
            weakEvaporation.Stop();
            strongEvaporation.Stop();
            bubbles.Stop();

            liquidSprite.enabled = false;
            rainbowRoot.gameObject.SetActive(false);
            vialTransform.localRotation = Quaternion.identity;

            SoundManager.Instance.PlayEffect(SoundEffect.VialEvaporated);
            SoundManager.Instance.StopEffect(bubblingAudioSource);
            SoundManager.Instance.StopEffect(evaporatingAudioSource);

            Sequence seq = DOTween.Sequence();

            if (tile.Marked)
            {
                liquidEvap.Play();
                neckEvap.Play();

                seq.Join(glowSprite.DOFade(0, glowFadeDuration));
                seq.Join(tileHighlight.DOFade(0, glowFadeDuration));
            }
            else
            {
                liquidEvap.Play();

                seq.Join(glowSprite.DOFade(0, glowFadeDuration));
                seq.Join(tileHighlight.DOFade(0, glowFadeDuration));
                seq.Join(PulseVial());
            }

            return seq;
        }

        public Tween Refill()
        {
            liquidSprite.enabled = true;

            return DOTween.Sequence();
        }

        public Tween MakeWildcard()
        {
            rainbowRoot.gameObject.SetActive(true);

            SetParticleColors(rainbowColors);

            SoundManager.Instance.PlayEffect(SoundEffect.WildcardCreated);

            return DOTween.Sequence();
        }

        public Tween FallToCurrentPosition(Vector2Int from)
        {
            float fallDelay = tile.Position.y * staggeredFallingDelay;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform
                .DOLocalMove((Vector3Int)tile.Position, Mathf.Sqrt((from.y - tile.Position.y) / fallingSpeed))
                .SetEase(fallingEase)
                .SetDelay(fallDelay));

            seq.AppendCallback(() => dust.Play());
            seq.AppendCallback(() => SoundManager.Instance.PlayEffect(SoundEffect.VialLanded));
            seq.Append(vialTransform.DOShakePosition(landingShakeDuration, landingShakeStrength * (from.y - tile.Position.y) / 9, landingShakeVibrato, landingShakeRandomness));

            return seq;
        }

        public Tween MoveToCurrentPosition(Vector2Int from)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(transform
                .DOLocalMove((Vector3Int)tile.Position, Mathf.Sqrt(2f * (from - tile.Position).magnitude / fallingSpeed))
                .SetEase(Ease.InOutQuad));

            SoundManager.Instance.PlayEffect(SoundEffect.VialShifted);

            return seq;
        }

        public Tween RotateToCurrentPosition(Vector2Int from, Vector2 pivot, RotationSense rotSense)
        {
            SoundManager.Instance.PlayEffect(SoundEffect.VialShifted);

            Sequence seq = DOTween.Sequence();

            #region Direct rotation

            //seq.Append(transform
            //    //.DOLocalMove((Vector3Int)tile.Position, Mathf.Sqrt(2f * (from - tile.Position).magnitude / fallingSpeed))
            //    .DOLocalMove((Vector3Int)tile.Position, 1f)
            //    .SetEase(Ease.InOutQuad));

            #endregion

            #region Manhattan rotation

            //Vector2Int to = tile.Position;
            //Vector2Int corner = Vector2Int.zero;
            //int cornerDistance = 0;
            //int fullDistance = 0;

            //if (rotSense == RotationSense.CW)
            //{
            //    // bottom to left
            //    if (to.x <= from.x && to.y >= from.y)
            //    {
            //        corner = new Vector2Int(to.x, from.y);
            //        cornerDistance = from.x - to.x;
            //        fullDistance = cornerDistance + to.y - from.y;
            //    }
            //    // left to top
            //    else if (to.x >= from.x && to.y >= from.y)
            //    {
            //        corner = new Vector2Int(from.x, to.y);
            //        cornerDistance = to.y - from.y;
            //        fullDistance = cornerDistance + to.x - from.x;
            //    }
            //    // top to right
            //    else if (to.x >= from.x && to.y <= from.y)
            //    {
            //        corner = new Vector2Int(to.x, from.y);
            //        cornerDistance = to.x - from.x;
            //        fullDistance = cornerDistance + from.y - to.y;
            //    }
            //    // right to bottom
            //    else
            //    {
            //        corner = new Vector2Int(from.x, to.y);
            //        cornerDistance = from.y - to.y;
            //        fullDistance = cornerDistance + from.x - to.x;
            //    }
            //}
            //else
            //{
            //    // right to top
            //    if (to.x <= from.x && to.y >= from.y)
            //    {
            //        corner = new Vector2Int(from.x, to.y);
            //        cornerDistance = to.y - from.y;
            //        fullDistance = cornerDistance + from.x - to.x;
            //    }
            //    // bottom to right
            //    else if (to.x >= from.x && to.y >= from.y)
            //    {
            //        corner = new Vector2Int(to.x, from.y);
            //        cornerDistance = to.x - from.x;
            //        fullDistance = cornerDistance + to.y - from.y;
            //    }
            //    // left to bottom
            //    else if (to.x >= from.x && to.y <= from.y)
            //    {
            //        corner = new Vector2Int(from.x, to.y);
            //        cornerDistance = from.y - to.y;
            //        fullDistance = cornerDistance + to.x - from.x;
            //    }
            //    // top to left
            //    else
            //    {
            //        corner = new Vector2Int(to.x, from.y);
            //        cornerDistance = from.x - to.x;
            //        fullDistance = cornerDistance + from.y - to.y;
            //    }
            //}

            //seq.Append(transform
            //    //.DOLocalPath(new Vector3[] { (Vector2)corner, (Vector2)to }, 1f, PathType.Linear, PathMode.Ignore)
            //    //.DOLocalPath(new Vector3[] { (Vector2)corner, (Vector2)to }, Mathf.Sqrt(2f * fullDistance / fallingSpeed), PathType.Linear, PathMode.Ignore)
            //    .DOLocalPath(new Vector3[] { (Vector2)corner, (Vector2)to }, 2f * fullDistance / fallingSpeed, PathType.Linear, PathMode.Ignore)
            //    .SetEase(Ease.Linear));

            #endregion

            #region Arc rotation
            Transform originalParent = transform.parent;

            Transform parent = new GameObject().transform;
            parent.parent = originalParent;
            parent.localPosition = pivot;
            transform.parent = parent;

            float targetAngle = Vector2.SignedAngle(from - pivot, tile.Position - pivot);

            seq.Join(parent.DOLocalRotate(new Vector3(0, 0, targetAngle), 1f).SetEase(Ease.InOutQuad));
            seq.Join(transform.DOLocalRotate(new Vector3(0, 0, -targetAngle), 1f).SetEase(Ease.InOutQuad));
            seq.AppendCallback(() =>
            {
                transform.parent = originalParent;
                transform.localRotation = Quaternion.identity;
            });
            #endregion

            return seq;
        }

        public Tween MatchAndDestroy()
        {
            weakEvaporation.Stop();
            strongEvaporation.Stop();
            puff.Play();
            SoundManager.Instance.StopEffect(evaporatingAudioSource);
            SoundManager.Instance.StopEffect(bubblingAudioSource);
            SoundManager.Instance.PlayEffect(SoundEffect.VialMatched);
            Sequence seq = DOTween.Sequence();

            seq.Append(vialTransform.DOScale(0, matchShrinkDuration).SetEase(Ease.OutBack));
            seq.Join(tileHighlight.DOFade(0, matchShrinkDuration));
            seq.AppendCallback(() => Destroy(gameObject));

            return seq;
        }

        public Tween Destroy()
        {
            weakEvaporation.Stop();
            strongEvaporation.Stop();
            puff.Play();
            SoundManager.Instance.PlayEffect(SoundEffect.VialDestroyed);
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

        private Tween PulseVial()
        {
            return vialTransform.DOPunchScale(Vector3.one * clickPulseScale, clickPulseDuration, 0, 0);
        }
    }
}