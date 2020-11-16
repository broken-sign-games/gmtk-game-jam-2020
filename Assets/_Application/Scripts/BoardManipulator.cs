using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.UI;
using GMTKJam2020.Input;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK2020
{
    public class BoardManipulator : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private SerializableDictionaryBase<Tool, ToolButton> toolButtons = null;
        [SerializeField] private RotationButton rotate3x3Button = null;
        [SerializeField] private ToolData toolData = null;

        public Tool ActiveTool { get; private set; }

        private InputActions inputs;

        private Toolbox toolbox;

        private Simulator simulator;
        private Board board;
        private bool initialized = false;
        private bool predictionsFinalised = false;

        private bool isDragging = false;
        private Vector2Int draggingFrom;

        private void Awake()
        {
            inputs = new InputActions();

            inputs.Gameplay.Select.performed += OnSelect;
            inputs.Gameplay.Select.canceled += OnRelease;

            ActiveTool = Tool.ToggleMarked;
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

        private void Update()
        {
            if (isDragging)
                OnDrag();
        }

        public void Initialize(Board initialBoard, Simulator simulator)
        {
            board = initialBoard;
            this.simulator = simulator;

            toolbox = new Toolbox(toolData, simulator);

            initialized = true;

            UpdateUI();
        }

        public void ToggleTool(ToolButton toolButton)
            => ToggleTool(toolButton.Tool);

        public void ToggleTool(Tool tool)
        {
            if (tool == ActiveTool)
                ActiveTool = Tool.ToggleMarked;
            else
                ActiveTool = tool;

            UpdateUI();
        }

        public void RewardMatch(MatchStep matchStep)
        {
            toolbox.RewardMatches(matchStep);

            UpdateUI();
        }

        public void LockPredictions()
        {
            predictionsFinalised = true;

            gameObject.SetActive(false);
        }

        public void UnlockPredictions()
        {
            predictionsFinalised = false;

            gameObject.SetActive(true);
        }

        private void OnSelect(InputAction.CallbackContext obj)
        {
            if (!initialized || predictionsFinalised)
                return;

            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();

            Vector2Int? gridPosOrNull = ActiveTool == Tool.Rotate2x2
                ? boardRenderer.PixelSpaceToHalfGridCoordinates(pointerPos)
                : boardRenderer.PixelSpaceToGridCoordinates(pointerPos);

            if (gridPosOrNull is null)
                return;

            Vector2Int gridPos = gridPosOrNull.Value;

            if (ActiveTool == Tool.SwapTiles || ActiveTool == Tool.SwapLines)
            {
                isDragging = true;
                draggingFrom = gridPos;
            }
            else
                UseActiveTool(gridPos);
        }

        private void OnDrag()
        {
            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();

            Vector2Int? gridPosOrNull = boardRenderer.PixelSpaceToGridCoordinates(pointerPos);

            if (gridPosOrNull is null)
            {
                isDragging = false;
                return;
            }

            Vector2Int gridPos = gridPosOrNull.Value;

            int delta = (gridPos - draggingFrom).sqrMagnitude;

            if (delta == 0)
                return;

            isDragging = false;

            if (delta > 1)
                return;

            UseSwapTool(draggingFrom, gridPos);
        }

        private void OnRelease(InputAction.CallbackContext obj)
        {
            if (!initialized || predictionsFinalised)
                return;

            isDragging = false;
        }

        private void UseActiveTool(Vector2Int gridPos)
        {
            // TODO: Indicate this error to the user
            if (toolbox.GetAvailableUses(ActiveTool) == 0)
                return;

            try
            {
                SimulationStep step;
                if (ActiveTool == Tool.Rotate3x3)
                    step = toolbox.UseTool(ActiveTool, gridPos, rotate3x3Button.RotationSense);
                else
                    step = toolbox.UseTool(ActiveTool, gridPos);

                KickOffAnimation(step);
                ActiveTool = Tool.ToggleMarked;

                UpdateUI();
            }
            catch (InvalidOperationException)
            {
                // TODO: Indicate error to the user
            }
        }

        private void UseSwapTool(Vector2Int from, Vector2Int to)
        {
            // TODO: Indicate this error to the user
            if (toolbox.GetAvailableUses(ActiveTool) == 0)
                return;

            try
            {
                SimulationStep step = toolbox.UseSwapTool(ActiveTool, from, to);

                KickOffAnimation(step);
                ActiveTool = Tool.ToggleMarked;

                UpdateUI();
            }
            catch (InvalidOperationException)
            {
                // TODO: Indicate error to the user
            }
        }

        private async void KickOffAnimation(SimulationStep step)
        {
            await boardRenderer.AnimateSimulationStepAsync(step);
        }

        private void UpdateUI()
        {
            foreach ((Tool tool, ToolButton button) in toolButtons)
            {
                button.UpdateUses(toolbox.GetAvailableUses(tool));
                int awarded = toolbox.GetAwardedToolsForCurrentChainLength(tool);
                int chainLength = toolbox.GetRequiredChainLength(tool);
                int available = toolbox.GetAvailableToolUsesForChainLength(chainLength);
                button.UpdateChainLength(
                    awarded,
                    available,
                    chainLength);
                button.UpdateActive(tool == ActiveTool);
            }
        }
    }
}