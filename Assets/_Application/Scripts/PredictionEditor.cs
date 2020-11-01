using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTKJam2020.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK2020
{
    public class PredictionEditor : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private ToolManager toolManager = null;

        private InputActions inputs;

        private Board board;
        private bool initialized = false;
        private bool predictionsFinalised = false;

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
            if (!initialized || predictionsFinalised || toolManager.ActiveTool.HasValue || toolManager.InputHandledThisFrame)
                return;

            Vector2 pointerPos = inputs.Gameplay.Point.ReadValue<Vector2>();
            Vector2Int? gridPosOrNull = boardRenderer.PixelSpaceToGridCoordinates(pointerPos);

            if (gridPosOrNull is null)
                return;

            Vector2Int gridPos = gridPosOrNull.Value;

            TogglePrediction(gridPos);
        }

        public void Initialize(Board initialBoard)
        {
            board = initialBoard;

            initialized = true;
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