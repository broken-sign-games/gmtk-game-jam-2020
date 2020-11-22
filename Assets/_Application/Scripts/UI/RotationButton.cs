using GMTK2020.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class RotationButton : ToolButton
    {
        [SerializeField] private BoardManipulator boardManipulator = null;
        [SerializeField] private Image rotationSenseIndicator = null;
        [SerializeField] private Sprite cwSprite = null;
        [SerializeField] private Sprite ccwSprite = null;
        
        public RotationSense RotationSense { get; private set; } = RotationSense.CCW;

        private void Start()
        {
            UpdateRotationSenseIndicator();
        }

        private void Update()
        {
            // HACK!
            if (boardManipulator.ActiveTool != Tool)
            {
                RotationSense = RotationSense.CCW;
                UpdateRotationSenseIndicator();
            }
        }

        public void OnClick()
        {
            if (boardManipulator.ActiveTool == Tool)
            {
                RotationSense = RotationSense.Other();
                UpdateRotationSenseIndicator();
            }

            if (RotationSense == RotationSense.CCW)
                boardManipulator.ToggleTool(Tool);
        }

        private void UpdateRotationSenseIndicator()
        {
            rotationSenseIndicator.sprite = RotationSense == RotationSense.CW ? cwSprite : ccwSprite;
        }
    }
}