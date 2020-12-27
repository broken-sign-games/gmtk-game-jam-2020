using GMTK2020.Audio;
using GMTK2020.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class RotationButton : ToolButton
    {
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
            if (BoardManipulator.ActiveTool != Tool)
            {
                RotationSense = RotationSense.CCW;
                UpdateRotationSenseIndicator();
            }
        }

        public override void OnClick()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.ToolSelected);

            if (BoardManipulator.ActiveTool == Tool)
            {
                RotationSense = RotationSense.Other();
                UpdateRotationSenseIndicator();
            }

            if (RotationSense == RotationSense.CCW)
                BoardManipulator.ToggleTool(Tool);
        }

        private void UpdateRotationSenseIndicator()
        {
            rotationSenseIndicator.sprite = RotationSense == RotationSense.CW ? cwSprite : ccwSprite;
        }
    }
}