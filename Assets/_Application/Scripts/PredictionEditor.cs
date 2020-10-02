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

        private Board initialBoard;
        private int[,] rawPredictions;
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

            IncrementPrediction(gridPos);
        }

        public void Initialize(Board initialBoard)
        {
            this.initialBoard = initialBoard;

            width = initialBoard.Width;
            height = initialBoard.Height;

            rawPredictions = new int[width, height];
            initialized = true;
        }

        public Prediction GetPredictions()
        {
            predictionsFinalised = true;

            var predictions = new Prediction();

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    int step = rawPredictions[x, y];
                    if (step > 0)
                        predictions.MatchedTilesPerStep[step - 1].Add(initialBoard[x, y]);
                }

            gameObject.SetActive(false);

            return predictions;
        }

        private void SetPrediction(Vector2Int pos, int value)
        {
            if (value > Simulator.MAX_SIMULATION_STEPS)
                return;

            rawPredictions[pos.x, pos.y] = value;

            boardRenderer.UpdatePrediction(pos, value);
        }

        private void IncrementPrediction(Vector2Int pos)
        {
            ++rawPredictions[pos.x, pos.y];
            rawPredictions[pos.x, pos.y] %= Simulator.MAX_SIMULATION_STEPS + 1;

            boardRenderer.UpdatePrediction(pos, rawPredictions[pos.x, pos.y]);
        }

        private void DecrementPrediction(Vector2Int pos)
        {
            rawPredictions[pos.x, pos.y] += Simulator.MAX_SIMULATION_STEPS;
            rawPredictions[pos.x, pos.y] %= Simulator.MAX_SIMULATION_STEPS + 1;

            boardRenderer.UpdatePrediction(pos, rawPredictions[pos.x, pos.y]);
        }
    }
}