using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.TutorialSystem;
using GMTK2020.UI;
using GMTK2020.Input;
using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GMTK2020
{
    public class BoardManipulator : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private SessionMetrics sessionMetrics = null;
        [SerializeField] private TutorialGridMaskManager tutorialOverlay = null;
        [SerializeField] private SerializableDictionaryBase<Tool, ToolButton> toolButtons = null;
        [SerializeField] private RotationButton rotate3x3Button = null;
        [SerializeField] private ToolData toolData = null;
        [SerializeField] private Transform reference00 = null;
        [SerializeField] private Transform reference11 = null;

        public Tool ActiveTool { get; private set; }
        public event Action LastToolUsed;
        public event Action<Tool> ActiveToolChanged;
        
        private InputActions inputs;

        private Toolbox toolbox;

        private bool initialized = false;
        private bool predictionsFinalised = false;

        private bool isDragging = false;
        private Vector2Int draggingFrom;

        private int width;
        private int height;

        private void Awake()
        {
            inputs = new InputActions();

            inputs.Gameplay.Select.started += OnSelect;
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
            inputs.Gameplay.Select.canceled -= OnRelease;
        }

        private void Update()
        {
            if (isDragging)
                OnDrag();
        }

        public void Initialize(Simulator simulator, Board board)
        {
            width = board.Width;
            height = board.Height;

            toolbox = new Toolbox(toolData, simulator);

            initialized = true;

            UpdateUI();
        }

        public void ToggleTool(Tool tool)
        {
            if (tool == ActiveTool)
                ActiveTool = Tool.ToggleMarked;
            else
                ActiveTool = tool;

            UpdateUI();

            ActiveToolChanged?.Invoke(ActiveTool);

            if (tool == ActiveTool)
                TutorialManager.Instance.CompleteActiveTutorial();
        }

        public bool AnyToolsAvailable() 
            => toolbox.AnyToolsAvailable();

        public async Task RewardMatches(MatchStep matchStep)
        {
            List<Tool> newTools = toolbox.RewardMatches(matchStep);

            if (newTools.Count > 0)
            {
                sessionMetrics.RegisterToolRewards(newTools);
                await ShowMatchShapeTutorialAsync(matchStep, newTools);
            }

            UpdateUI();
        }

        private async Task ShowMatchShapeTutorialAsync(MatchStep matchStep, List<Tool> newTools)
        {
            var matchedRects = matchStep.LeftEndsOfHorizontalMatches
                .Select(pos => new GridRect(pos, pos + new Vector2Int(2, 0)))
                .Concat(matchStep.BottomEndsOfVerticalMatches
                    .Select(pos => new GridRect(pos, pos + new Vector2Int(0, 2))))
                .ToList();

            await TutorialManager.Instance.ShowTutorialIfNewAsync(TutorialID.MatchShapes, matchedRects, newTools);
        }

        public void MakeToolsAvailable()
        { 
            toolbox.MakeToolsAvailable();

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

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            if (!initialized || predictionsFinalised)
                return;

            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();

            Vector2Int? gridPosOrNull = ActiveTool == Tool.Rotate2x2
                ? PixelSpaceToHalfGridCoordinates(pointerPos)
                : PixelSpaceToGridCoordinates(pointerPos);

            if (gridPosOrNull is null)
                return;

            Vector2Int gridPos = gridPosOrNull.Value;

            if (!tutorialOverlay.IsPositionAllowedByCurrentMask(gridPos))
                return;

            if (ActiveTool == Tool.SwapTiles || ActiveTool == Tool.SwapLines)
            {
                isDragging = true;
                draggingFrom = gridPos;
                boardRenderer.IndicateStartOfSwap(gridPos);
            }
            else
                UseActiveTool(gridPos);
        }

        private void OnDrag()
        {
            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();

            Vector2Int? gridPosOrNull = PixelSpaceToGridCoordinates(pointerPos);

            if (gridPosOrNull is null)
            {
                isDragging = false;
                return;
            }

            Vector2Int gridPos = gridPosOrNull.Value;

            if (!tutorialOverlay.IsPositionAllowedByCurrentMask(gridPos))
            {
                isDragging = false;
                return;
            }

            int delta = (gridPos - draggingFrom).sqrMagnitude;

            if (delta == 0)
                return;

            isDragging = false;

            if (delta > 1)
                return;

            UseSwapTool(draggingFrom, gridPos);
        }

        private void OnRelease(InputAction.CallbackContext ctx)
        {
            if (!initialized || predictionsFinalised)
                return;

            if (isDragging)
                boardRenderer.StopIndicatingSwap(draggingFrom);

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

                sessionMetrics.RegisterToolUse(ActiveTool);

                KickOffAnimation(step);
                ActiveTool = Tool.ToggleMarked;
                ActiveToolChanged?.Invoke(ActiveTool);

                UpdateUI();
            }
            catch (InvalidOperationException)
            {
                // TODO: Indicate error to the user
            }
        }

        private void UseSwapTool(Vector2Int from, Vector2Int to)
        {
            boardRenderer.StopIndicatingSwap(from);

            // TODO: Indicate this error to the user
            if (toolbox.GetAvailableUses(ActiveTool) == 0)
                return;

            try
            {
                SimulationStep step = toolbox.UseSwapTool(ActiveTool, from, to);

                sessionMetrics.RegisterToolUse(ActiveTool);

                KickOffAnimation(step);
                ActiveTool = Tool.ToggleMarked;
                ActiveToolChanged?.Invoke(ActiveTool);

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

            if (!AnyToolsAvailable())
                LastToolUsed?.Invoke();
        }

        public void UpdateUI()
        {
            foreach ((Tool tool, ToolButton button) in toolButtons)
            {
                button.UpdateUses(toolbox.GetAvailableUses(tool));
                button.UpdateAvailable(toolbox.IsToolAvailable(tool));
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

        private Vector2Int? PixelSpaceToGridCoordinates(Vector3 mousePosition)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector3 localPos = worldPos - reference00.position;
            Vector3 referenceScale = reference11.position - reference00.position;

            var gridPos = new Vector2Int(Mathf.RoundToInt(localPos.x / referenceScale.x), Mathf.RoundToInt(localPos.y / referenceScale.y));

            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width || gridPos.y >= height)
                return null;

            return gridPos;
        }

        public Vector2Int? PixelSpaceToHalfGridCoordinates(Vector2 mousePosition)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector3 localPos = worldPos - reference00.position;
            Vector3 referenceScale = reference11.position - reference00.position;

            var gridPos = new Vector2Int(Mathf.RoundToInt(localPos.x / referenceScale.x - 0.5f), Mathf.RoundToInt(localPos.y / referenceScale.y - 0.5f));

            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width - 1 || gridPos.y >= height - 1)
                return null;

            return gridPos;
        }
    }
}