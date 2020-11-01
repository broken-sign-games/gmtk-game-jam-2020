using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTKJam2020.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GMTK2020
{
    public class ToolManager : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer;
        [SerializeField] private Button removeTileButton;

        private InputActions inputs;

        private Simulator simulator;

        public Tool? ActiveTool;
        public bool InputHandledThisFrame;

        private void Awake()
        {
            inputs = new InputActions();

            inputs.Gameplay.Select.performed += OnSelect;
        }

        private void OnEnable()
        {
            inputs.Enable();
        }

        private void OnDisable()
        {
            inputs.Disable();
        }

        private void OnDestroy()
        {
            inputs.Gameplay.Select.performed -= OnSelect;
        }

        private void OnSelect(InputAction.CallbackContext obj)
        {
            if (!ActiveTool.HasValue)
                return;

            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();
            Vector2Int? gridPosOrNull = boardRenderer.PixelSpaceToGridCoordinates(pointerPos);

            if (gridPosOrNull is null)
                return;

            Vector2Int gridPos = gridPosOrNull.Value;

            UseTool(ActiveTool.Value, gridPos);
        }

        private void UseTool(Tool tool, Vector2Int gridPos)
        {
            switch (tool)
            {
            case Tool.RemoveTile:
                RemovalStep step = simulator.RemoveTile(gridPos);
                KickOffAnimation(step);
                break;
            case Tool.RefillInert:
                break;
            case Tool.Bomb:
                break;
            case Tool.RemoveRow:
                break;
            case Tool.RemoveColor:
                break;
            case Tool.ShuffleBoard:
                break;
            case Tool.RotateBoard:
                break;
            case Tool.Rotate2x2:
                break;
            case Tool.Rotate3x3:
                break;
            case Tool.CreateWildcard:
                break;
            case Tool.SwapTiles:
                break;
            case Tool.SwapLines:
                break;
            default:
                break;
            }

            ActiveTool = null;
            InputHandledThisFrame = true;
            UpdateUI();
        }

        private void LateUpdate()
        {
            InputHandledThisFrame = false;
        }

        private async void KickOffAnimation(SimulationStep step)
        {
            await boardRenderer.AnimateSimulationStepAsync(step);
        }

        public void SetSimulator(Simulator simulator)
        {
            this.simulator = simulator;
        }

        public void ToggleTool(ToolHolder toolHolder)
        {
            Tool tool = toolHolder.Tool;
            if (tool == ActiveTool)
                ActiveTool = null;
            else
                ActiveTool = tool;

            UpdateUI();
        }

        private void UpdateUI()
        {
            ColorBlock colors = removeTileButton.colors;
            Color color = ActiveTool == Tool.RemoveTile ? Color.grey : Color.white;
            colors.normalColor = color;
            colors.selectedColor = color;
            removeTileButton.colors = colors;
        }
    }
}