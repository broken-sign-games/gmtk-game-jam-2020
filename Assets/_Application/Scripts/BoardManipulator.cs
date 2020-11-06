using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTKJam2020.Input;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GMTK2020
{
    public class BoardManipulator : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private SerializableDictionaryBase<Tool, Button> toolButtons = null;

        private InputActions inputs;

        private Simulator simulator;
        private Tool activeTool;
        private Board board;
        private bool initialized = false;
        private bool predictionsFinalised = false;

        private void Awake()
        {
            inputs = new InputActions();

            inputs.Gameplay.Select.performed += OnSelect;
            activeTool = Tool.ToggleMarked;
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

        public void Initialize(Board initialBoard, Simulator simulator)
        {
            board = initialBoard;
            this.simulator = simulator;

            initialized = true;
        }

        public void UseTool(ToolHolder toolHolder)
        {
            SimulationStep step = null;

            switch (toolHolder.Tool)
            {
            case Tool.ShuffleBoard:
                step = simulator.ShuffleBoard();
                break;
            }

            if (step != null)
                KickOffAnimation(step);

            activeTool = Tool.ToggleMarked;
            UpdateUI();
        }

        public void ToggleTool(ToolHolder toolHolder)
        {
            Tool tool = toolHolder.Tool;
            if (tool == activeTool)
                activeTool = Tool.ToggleMarked;
            else
                activeTool = tool;

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
            Vector2Int? gridPosOrNull = boardRenderer.PixelSpaceToGridCoordinates(pointerPos);

            if (gridPosOrNull is null)
                return;

            Vector2Int gridPos = gridPosOrNull.Value;

            UseActiveTool(gridPos);
        }

        private void UseActiveTool(Vector2Int gridPos)
        {
            SimulationStep step = null;

            switch (activeTool)
            {
            case Tool.ToggleMarked:
                TogglePrediction(gridPos);
                break;
            case Tool.RemoveTile:
                step = simulator.RemoveTile(gridPos);
                break;
            case Tool.RefillInert:
                break;
            case Tool.Bomb:
                step = simulator.RemoveBlock(gridPos);
                break;
            case Tool.RemoveRow:
                step = simulator.RemoveRow(gridPos.y);
                break;
            case Tool.RemoveColor:
                step = simulator.RemoveColor(board[gridPos].Color);
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
            }

            if (step != null)
                KickOffAnimation(step);

            activeTool = Tool.ToggleMarked;
            UpdateUI();
        }

        private async void KickOffAnimation(SimulationStep step)
        {
            await boardRenderer.AnimateSimulationStepAsync(step);
        }

        private void UpdateUI()
        {
            foreach ((Tool tool, Button button) in toolButtons)
                UpdateButtonColor(button, tool);
        }

        private void UpdateButtonColor(Button button, Tool tool)
        {
            ColorBlock colors = button.colors;
            Color color = activeTool == tool ? Color.grey : Color.white;
            colors.normalColor = color;
            colors.selectedColor = color;
            button.colors = colors;
        }

        private void TogglePrediction(Vector2Int pos)
        {
            Tile tile = board[pos];

            // TODO: Play a sound effect, maybe do a little animation on the vial
            if (tile.Inert)
                return;

            tile.Marked = !tile.Marked;

            boardRenderer.UpdatePrediction(tile);
        }
    }
}