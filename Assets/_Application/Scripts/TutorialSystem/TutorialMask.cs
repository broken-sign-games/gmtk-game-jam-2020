using GMTK2020.Data;
using UnityEngine;

namespace GMTK2020.TutorialSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TutorialMask : MonoBehaviour
    {
        [SerializeField] private SpriteMask overlayMask = null;
        [SerializeField] private SpriteMask interactionMask = null;

        [SerializeField] private int pixelsPerUnit = 256;
        [SerializeField] private int extrudeOverlayBy = 24;

        private SpriteRenderer featheredWindowSprite;
        private float extraSize;

        private void Awake()
        {
            featheredWindowSprite = GetComponent<SpriteRenderer>();

            extraSize = extrudeOverlayBy / (float)pixelsPerUnit;
        }

        public void SetGridRect(GridRect rect)
        {
            transform.localPosition = rect.Center;
            featheredWindowSprite.size = new Vector2(rect.Width + extraSize, rect.Height + extraSize);
            overlayMask.transform.localScale = new Vector3(rect.Width + extraSize, rect.Height + extraSize, 1f);
            interactionMask.transform.localScale = new Vector3(rect.Width, rect.Height, 1f);
        }

        public void SetWorldSpaceRect(Rect rect)
        {
            transform.position = rect.center;
            featheredWindowSprite.size = rect.size + extraSize * Vector2.one;
            overlayMask.transform.localScale = new Vector3(rect.width + extraSize, rect.height + extraSize, 1f);
            interactionMask.transform.localScale = new Vector3(rect.width, rect.height, 1f);
        }
    } 
}
