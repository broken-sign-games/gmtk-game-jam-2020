using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class ChainCounter : MonoBehaviour
    {
        [SerializeField] private Transform chainSegmentRoot = null;
        [SerializeField] private Transform spikeBallRoot = null;
        [SerializeField] private RectTransform spikeBallPrefab = null;
        [SerializeField] private ChainSegment chainSegmentPrefab = null;
        [SerializeField] private float segmentWidth = 144f;
        [SerializeField] private float spikeBallOffset = 72f;
        [SerializeField] private int maxVisibleChains = 6;

        private int maxCracks;
        private int currentChainLength;

        private ChainSegment lastSegment;

        public void SetMaxCracks(int maxCracks)
        {
            this.maxCracks = maxCracks;
        }

        public void AddChain()
        {
            if (spikeBallRoot.childCount > 0)
                Destroy(spikeBallRoot.GetChild(0).gameObject);

            if (lastSegment)
                lastSegment.AddShadow();

            ChainSegment newSegment = Instantiate(chainSegmentPrefab, chainSegmentRoot);
            RectTransform rectTransform = newSegment.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(currentChainLength * segmentWidth, 0);
            lastSegment = newSegment;

            ++currentChainLength;
        }

        public void ResetChain()
        {
            foreach (Transform segment in chainSegmentRoot)
                Destroy(segment.gameObject);

            foreach (Transform spikeBall in spikeBallRoot)
                Destroy(spikeBall.gameObject);

            float xPos = spikeBallOffset;

            for (int i = 0; i < maxCracks; ++i)
            {
                RectTransform spikeBall = Instantiate(spikeBallPrefab, spikeBallRoot);
                spikeBall.anchoredPosition = new Vector2(xPos, 0);
                xPos += segmentWidth;
            }

            lastSegment = null;
            currentChainLength = 0;
        }

        private void UpdateUI()
        {
            // crackDisplay.text = new string('0', avoidedCracks) + new string('O', maxCracks - avoidedCracks);
        }
    } 
}
