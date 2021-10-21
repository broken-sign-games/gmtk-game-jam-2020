using DG.Tweening;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class ChainCounter : MonoBehaviour
    {
        [SerializeField] private RectTransform chainSegmentRoot = null;
        [SerializeField] private ChainSegment chainSegmentPrefab = null;
        [SerializeField] private TextMeshProUGUI resourceString = null;
        [SerializeField] private float segmentWidth = 144f;
        [SerializeField] private int maxVisibleChains = 6;
        [SerializeField] private float chainSlideSpeed = 5;
        
        private int currentChainLength;

        private ChainSegment lastSegment;

        private int currentResource;
        private int initialResource;

        public void RenderInitialResource(int initialResource)
        {
            this.initialResource = initialResource;
            this.currentResource = initialResource;
            UpdateResource();
        }

        public void RenderInitialChain()
        {
            lastSegment = null;
            currentChainLength = 0;
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

            lastSegment = newSegment;
            ++currentChainLength;

            return seq;
        }

        public Tween ResetChain()
        {
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

            seq.AppendCallback(() =>
            {
                lastSegment = null;
                currentChainLength = 0;
            });

            return seq;
        }

        public void RegisterChangeInResource(int delta)
        {
            currentResource += delta;
            currentResource = Math.Max(0, currentResource);
            UpdateResource();
        }

        private void UpdateResource()
        {
            resourceString.text = $"{currentResource} / {initialResource}";
        }
    }
}
