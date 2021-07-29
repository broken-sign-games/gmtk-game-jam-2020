using DG.Tweening;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class ChainCounter : MonoBehaviour
    {
        [SerializeField] private RectTransform chainSegmentRoot = null;
        [SerializeField] private RectTransform spikeBallRoot = null;
        [SerializeField] private RectTransform spikeBallPrefab = null;
        [SerializeField] private ChainSegment chainSegmentPrefab = null;
        [SerializeField] private float segmentWidth = 144f;
        [SerializeField] private float spikeBallOffset = 72f;
        [SerializeField] private int maxVisibleChains = 6;
        [SerializeField] private float spikeBallFlySpeed = 5;
        [SerializeField] private float chainSlideSpeed = 5;
        [SerializeField] private float spikeBallSpawnDelay = 0.25f;

        private int maxCracks;
        private int currentChainLength;
        private int nextSpikeBall;

        private ChainSegment lastSegment;

        private TutorialManager tutorialManager;

        private void Start()
        {
            tutorialManager = TutorialManager.Instance;

            tutorialManager.TutorialReady += OnTutorialReady;
            tutorialManager.TutorialCompleted += OnTutorialCompleted;
        }

        private void OnDestroy()
        {
            tutorialManager.TutorialReady -= OnTutorialReady;
            tutorialManager.TutorialCompleted -= OnTutorialCompleted;
        }

        public void SetMaxCracks(int maxCracks)
        {
            this.maxCracks = maxCracks;
        }

        public void RenderInitialChain()
        {
            float xPos = spikeBallOffset;

            for (int i = 0; i < maxCracks; ++i)
            {
                RectTransform spikeBall = Instantiate(spikeBallPrefab, spikeBallRoot);
                spikeBall.anchoredPosition = new Vector2(xPos, 0);
                // ewww
                Image spikeSprite = spikeBall.GetComponent<Image>();
                spikeSprite.SetAlpha(1);
                xPos += segmentWidth;
            }

            lastSegment = null;
            currentChainLength = 0;
            nextSpikeBall = 0;
        }

        public Tween AddChain()
        {
            Sequence seq = DOTween.Sequence();

            if (currentChainLength >= maxVisibleChains)
            {
                float chainTargetPos = (maxVisibleChains - currentChainLength - 1) * segmentWidth;
                float chainMoveDistance = chainSegmentRoot.anchoredPosition.x - chainTargetPos;

                seq.Append(chainSegmentRoot
                    .DOAnchorPosX(chainTargetPos, chainMoveDistance / chainSlideSpeed * 2));
            }

            ChainSegment newSegment = Instantiate(chainSegmentPrefab, chainSegmentRoot);
            RectTransform rectTransform = newSegment.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(currentChainLength * segmentWidth, 0);

            seq.Append(newSegment.AnimateAppearance(lastSegment));

            if (spikeBallRoot.childCount > 0)
            {
                var spikeBall = spikeBallRoot.GetChild(0).GetComponent<SpikeBall>();
                seq.Append(spikeBall.AnimateDestruction());
            }

            lastSegment = newSegment;
            ++currentChainLength;

            return seq;
        }

        public Tween SendSpikeBall(Vector3 targetPosition)
        {
            Sequence seq = DOTween.Sequence();

            Transform spikeBall = spikeBallRoot.GetChild(nextSpikeBall);

            seq.Append(spikeBall
                .DOMove(targetPosition, Vector3.Distance(spikeBall.position, targetPosition) / spikeBallFlySpeed)
                .SetEase(Ease.InQuad));
            seq.AppendCallback(() => Destroy(spikeBall.gameObject));

            ++nextSpikeBall;

            return seq;
        }

        public Tween ResetChain()
        {
            // This can happen when there are fewer unbroken/nonempty vials than spike balls.
            foreach (Transform spikeBall in spikeBallRoot)
                Destroy(spikeBall.gameObject);

            Sequence seq = DOTween.Sequence();

            float chainTargetPos = -currentChainLength * segmentWidth;
            float chainMoveDistance = chainSegmentRoot.anchoredPosition.x - chainTargetPos;

            seq.Append(chainSegmentRoot
                .DOAnchorPosX(chainTargetPos, chainMoveDistance / chainSlideSpeed)
                .SetEase(Ease.InQuad));

            seq.AppendCallback(() =>
            {
                foreach (Transform segment in chainSegmentRoot)
                    Destroy(segment.gameObject);

                chainSegmentRoot.anchoredPosition *= new Vector2(0, 1);
            });

            float xPos = spikeBallOffset;
            float delay = seq.Duration() + spikeBallSpawnDelay;

            for (int i = 0; i < maxCracks; ++i)
            {
                float _xPos = xPos;

                var spikeBall = Instantiate(spikeBallPrefab, spikeBallRoot).GetComponent<SpikeBall>();
                seq.Insert(delay, spikeBall.AnimateAppearance(_xPos));

                xPos += segmentWidth;
                delay += spikeBallSpawnDelay;
            }

            seq.AppendCallback(() =>
            {
                lastSegment = null;
                currentChainLength = 0;
                nextSpikeBall = 0;
            });

            return seq;
        }

        private Task OnTutorialReady(Tutorial tutorial)
        {
            if (!tutorial.HighlightSpikeBalls)
                return Task.CompletedTask;

            foreach (var spikeBall in spikeBallRoot.GetComponentsInChildren<SpikeBall>())
                spikeBall.EnableMask();

            return Task.CompletedTask;
        }

        private Task OnTutorialCompleted(Tutorial tutorial)
        {
            if (!tutorial.HighlightSpikeBalls)
                return Task.CompletedTask;

            foreach (var spikeBall in spikeBallRoot.GetComponentsInChildren<SpikeBall>())
                spikeBall.DisableMask();

            return Task.CompletedTask;
        }
    }
}
