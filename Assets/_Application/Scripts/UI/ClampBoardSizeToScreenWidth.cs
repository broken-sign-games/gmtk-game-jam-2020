using UnityEngine;

namespace GMTK2020.UI
{
    public class ClampBoardSizeToScreenWidth : MonoBehaviour
    {
        [SerializeField] private RectTransform infoBox = null;
        [SerializeField] private float maxBoardWidth = 1024f;
        [SerializeField] private float borderWidth = 20f;
        [SerializeField] private float minInfoBoxHeight = 280f;

        private Vector2Int resolution;

        private void Awake()
        {
            UpdateBoardSize();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
                UpdateBoardSize();
        }
#endif

        private void UpdateBoardSize()
        {
            resolution = new Vector2Int(Screen.width, Screen.height);

            RectTransform parent = transform.parent as RectTransform;
            float boardWidth = Mathf.Min(maxBoardWidth, parent.rect.width - 2 * borderWidth);

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(boardWidth, boardWidth);

            float infoBoxHeight = minInfoBoxHeight + (maxBoardWidth - boardWidth);
            infoBox.sizeDelta = new Vector2(infoBox.sizeDelta.x, infoBoxHeight);
        }
    } 
}
