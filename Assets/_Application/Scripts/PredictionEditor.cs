using GMTK2020.Data;
using GMTK2020.Rendering;
using UnityEngine;

namespace GMTK2020
{
    public class PredictionEditor : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;

        private Tile[,] initialGrid;
        private int[,] rawPredictions;
        private bool initialized = false;
        private bool predictionsFinalised = false;
        
        int width;
        int height;

        public void Initialize(Tile[,] initialGrid)
        {
            this.initialGrid = initialGrid.Clone() as Tile[,];

            width = initialGrid.GetLength(0);
            height = initialGrid.GetLength(1);

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
                        predictions.MatchedTilesPerStep[step - 1].Add(initialGrid[x, y]);
                }

            gameObject.SetActive(false);

            return predictions;
        }

        private void Update()
        {
            if (!initialized || predictionsFinalised)
                return;

            Vector2Int? gridPosOrNull = boardRenderer.PixelSpaceToGridCoordinates(Input.mousePosition);

            if (gridPosOrNull is null)
                return;

            var gridPos = (Vector2Int)gridPosOrNull;

            if (Input.GetMouseButtonDown(0))
                IncrementPrediction(gridPos);
            else if (Input.GetMouseButtonDown(1))
                DecrementPrediction(gridPos);
            else if (Input.anyKey
                && Input.inputString.Length == 1 
                && int.TryParse(Input.inputString, out int digit))
            {
                SetPrediction(gridPos, digit);
            }
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
            rawPredictions[pos.x, pos.y] %= Simulator.MAX_SIMULATION_STEPS+1;

            boardRenderer.UpdatePrediction(pos, rawPredictions[pos.x, pos.y]);
        }

        private void DecrementPrediction(Vector2Int pos)
        {
            rawPredictions[pos.x, pos.y] += Simulator.MAX_SIMULATION_STEPS;
            rawPredictions[pos.x, pos.y] %= Simulator.MAX_SIMULATION_STEPS+1;

            boardRenderer.UpdatePrediction(pos, rawPredictions[pos.x, pos.y]);
        }
    }
}