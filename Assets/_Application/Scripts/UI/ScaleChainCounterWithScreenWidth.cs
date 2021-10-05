using UnityEngine;

namespace GMTK2020.UI
{
    public class ScaleChainCounterWithScreenWidth : MonoBehaviour
    {
        [SerializeField] private float maxWidth = 900f;
        [SerializeField] private float rightBuffer = 180f;

        private Vector2Int resolution;

        private void Awake()
        {
            UpdateCounterWidth();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
                UpdateCounterWidth();
        }
#endif

        private void UpdateCounterWidth()
        {
            resolution = new Vector2Int(Screen.width, Screen.height);

            RectTransform parent = transform.parent as RectTransform;
            float targetWidth = Mathf.Min(maxWidth, parent.rect.width - rightBuffer);

            transform.localScale = new Vector3(targetWidth / maxWidth, targetWidth / maxWidth, 1);
        }
    } 
}
