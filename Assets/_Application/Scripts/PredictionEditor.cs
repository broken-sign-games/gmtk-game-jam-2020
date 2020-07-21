using GMTK2020.Data;
using GMTK2020.Rendering;
using UnityEngine;

namespace GMTK2020
{
    public class PredictionEditor : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;

        private Tile[,] initialGrid;
        private bool[,] rawPredictions;
        private bool initialized = false;
        private bool predictionsFinalised = false;

        int width;
        int height;

        public void Initialize(Tile[,] initialGrid)
        {
            this.initialGrid = initialGrid.Clone() as Tile[,];

            width = initialGrid.GetLength(0);
            height = initialGrid.GetLength(1);

            rawPredictions = new bool[width, height];
            initialized = true;
        }

        public Prediction GetPredictions()
        {
            predictionsFinalised = true;

            var predictions = new Prediction();

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    if (rawPredictions[x, y])
                        predictions.PredictedTiles.Add(initialGrid[x, y]);
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

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                TogglePrediction(gridPos);
        }

        private void TogglePrediction(Vector2Int pos)
        {
            rawPredictions[pos.x, pos.y] = !rawPredictions[pos.x, pos.y];

            boardRenderer.UpdatePrediction(pos, rawPredictions[pos.x, pos.y]);
        }
    }
}