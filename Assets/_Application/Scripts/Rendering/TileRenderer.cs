using DG.Tweening;
using GMTK2020.Audio;
using GMTK2020.Data;
using UnityEngine;

namespace GMTK2020.Rendering
{
    [RequireComponent(typeof(AudioSource))]
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
        [SerializeField] private GameObject wildcardIndicator = null;

        [SerializeField] private SpriteRenderer incorrectBackground = null;
        [SerializeField] private SpriteRenderer missingPredictionIndicator = null;
        [SerializeField] private ParticleSystem bubbles = null;
        [SerializeField] private ParticleSystem pop = null;
        [SerializeField] private ParticleSystem popRing = null;
        [SerializeField] private ParticleSystem puff = null;
        [SerializeField] private ParticleSystem liquidEvap = null;
        [SerializeField] private ParticleSystem neckEvap = null;
        [SerializeField] private ParticleSystem dust = null;

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
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

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

            vialMask.sprite = tileData.VialMaskMap[tile.Color];
            corkSprite.sprite = tileData.CorkSpriteMap[tile.Color];
            liquidSprite.sprite = tileData.LiquidSpriteMap[tile.Color];
            liquidMask.sprite = tileData.LiquidSpriteMap[tile.Color];
            glowSprite.sprite = tileData.GlowSpriteMap[tile.Color];
            glowSprite.color = tileData.GlowColor[tile.Color];

            UpdateCracks(false);

            Color highlightColor = tileData.PopDropletColor[tile.Color];
            highlightColor.a = 0;
            tileHighlight.color = highlightColor;

            ParticleSystem.MainModule mainPop = pop.main;
            mainPop.startColor = tileData.PopDropletColor[tile.Color];
            ParticleSystem.MainModule mainPopRing = popRing.main;
            mainPopRing.startColor = tileData.PopDropletColor[tile.Color];
            ParticleSystem.MainModule mainPuff = puff.main;
            mainPuff.startColor = tileData.PopDropletColor[tile.Color];
            ParticleSystem.MainModule mainLiquidEvap = liquidEvap.main;
            mainLiquidEvap.startColor = tileData.PopDropletColor[tile.Color];
            ParticleSystem.MainModule mainNeckEvap = neckEvap.main;
            mainNeckEvap.startColor = tileData.PopDropletColor[tile.Color];
        }

        public Tween UpdateCracks(bool playSound = true)
        {
            if (playSound)
                SoundManager.Instance.PlayEffect(SoundEffect.VialCracked);

            glassSprite.sprite = tileData.VialSpriteMap[tile.Color][tile.Cracks];

            return vialTransform.DOPunchScale(Vector3.one * clickPulseScale, clickPulseDuration, 0, 0);
        }

        public Tween UpdatePrediction()
        {
            if (tile.Marked)
            {
                SoundManager.Instance.PlayEffect(SoundEffect.VialOpened);
                SoundManager.Instance.StartPlayingLoopEffect(audioSource, SoundEffect.VialBubbling);
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
                SoundManager.Instance.PlayEffectWithRandomPitch(SoundEffect.VialClosed);
                audioSource.Stop();
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
            liquidEvap.Play();
            neckEvap.Play();
            bubbles.Stop();
            vialTransform.localRotation = Quaternion.identity;

            SoundManager.Instance.PlayEffect(SoundEffect.VialEvaporated);

            Sequence seq = DOTween.Sequence();

            seq.Join(glowSprite.DOFade(0, glowFadeDuration));
            seq.Join(tileHighlight.DOFade(0, glowFadeDuration));

            liquidSprite.enabled = false;
            wildcardIndicator.SetActive(false);

            return seq;
        }

        public Tween Refill()
        {
            liquidSprite.enabled = true;

            return DOTween.Sequence();
        }

        public Tween MakeWildcard()
        {
            wildcardIndicator.SetActive(true);

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
            puff.Play();
            audioSource.Stop();
            SoundManager.Instance.PlayEffect(SoundEffect.VialMatched);
            Sequence seq = DOTween.Sequence();

            seq.Append(vialTransform.DOScale(0, matchShrinkDuration).SetEase(Ease.OutBack));
            seq.Join(tileHighlight.DOFade(0, matchShrinkDuration));
            seq.AppendCallback(() => Destroy(gameObject));

            return seq;
        }

        public Tween Destroy()
        {
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