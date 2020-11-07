using GMTK2020.Data;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(ToolHolder))]
    public class RotationButton : MonoBehaviour
    {
        [SerializeField] private BoardManipulator boardManipulator;
        [SerializeField] private Image rotationSenseIndicator;
        [SerializeField] private Sprite cwSprite;
        [SerializeField] private Sprite ccwSprite;
        
        private Tool rotationTool;

        public RotationSense RotationSense { get; private set; } = RotationSense.CCW;

        private void Start()
        {
            rotationTool = GetComponent<ToolHolder>().Tool;

            UpdateRotationSenseIndicator();
        }

        private void Update()
        {
            // HACK!
            if (boardManipulator.ActiveTool != rotationTool)
            {
                RotationSense = RotationSense.CCW;
                UpdateRotationSenseIndicator();
            }
        }

        public void OnClick()
        {
            if (boardManipulator.ActiveTool == rotationTool)
            {
                RotationSense = RotationSense.Other();
                UpdateRotationSenseIndicator();
            }

            if (RotationSense == RotationSense.CCW)
                boardManipulator.ToggleTool(rotationTool);
        }

        private void UpdateRotationSenseIndicator()
        {
            rotationSenseIndicator.sprite = RotationSense == RotationSense.CW ? cwSprite : ccwSprite;
        }
    }
}