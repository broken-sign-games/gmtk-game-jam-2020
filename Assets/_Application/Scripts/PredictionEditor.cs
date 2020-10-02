using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTKJam2020.Input;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK2020
{
    public class PredictionEditor : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;

        private InputActions inputs;

        private Board board;
        private bool initialized = false;
        private bool predictionsFinalised = false;

        int width;
        int height;

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
            if (!initialized || predictionsFinalised)
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

        public Prediction GetPredictions()
        {
            predictionsFinalised = true;

            gameObject.SetActive(false);

            return new Prediction();
        }

        private void TogglePrediction(Vector2Int pos)
        {
            Tile tile = board[pos];
            tile.Marked = !tile.Marked;

            boardRenderer.UpdatePrediction(tile);
        }
    }
}