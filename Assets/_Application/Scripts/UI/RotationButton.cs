using GMTK2020.Audio;
using GMTK2020.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class RotationButton : ToolButton
    {
        [SerializeField] private Image rotationSenseIndicator = null;
        [SerializeField] private Sprite enabledCWSprite = null;
        [SerializeField] private Sprite disabledCWSprite = null;
        [SerializeField] private Sprite enabledCCWSprite = null;
        [SerializeField] private Sprite disabledCCWSprite = null;
        
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

        public override void UpdateAvailable(bool available)
        {
            base.UpdateAvailable(available);

            UpdateRotationSenseIndicator();
        }

        private void UpdateRotationSenseIndicator()
        {
            if (Available)
                rotationSenseIndicator.sprite = RotationSense == RotationSense.CW ? enabledCWSprite : enabledCCWSprite;
            else
                rotationSenseIndicator.sprite = RotationSense == RotationSense.CW ? disabledCWSprite : disabledCCWSprite;
        }
    }
}