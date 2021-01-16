using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK2020.UI
{
    public class ToolPanel : MonoBehaviour
    {
        private Dictionary<Tool, ToolButton> toolToButton;
        private Dictionary<Tool, int> toolToIndex;
        private List<ToolButton> toolButtons;

        private void Awake()
        {
            toolToButton = new Dictionary<Tool, ToolButton>();
            toolToIndex = new Dictionary<Tool, int>();
            toolButtons = new List<ToolButton>();

            foreach (Transform child in transform)
            {
                var toolButton = child.GetComponent<ToolButton>();
                if (toolButton && toolButton.gameObject.activeSelf)
                {
                    toolToButton[toolButton.Tool] = toolButton;
                    toolToIndex[toolButton.Tool] = toolButtons.Count;
                    toolButtons.Add(toolButton);
                }
            }
        }

        public Vector2[] GetButtonCornersInWorldSpace(Tool tool)
        {
            var rectTransform = toolToButton[tool].GetComponent<RectTransform>();

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            return corners
                .Select(corner => (Vector2)corner)
                .ToArray();
        }
    } 
}
