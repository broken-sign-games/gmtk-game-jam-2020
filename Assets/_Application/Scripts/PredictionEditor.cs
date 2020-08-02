using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.UI;
using System;
using UnityEngine;

namespace GMTK2020
{
    public class PredictionEditor : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private MouseEventSource mouseEventSource = null;

        private Tile[,] grid;
        private bool initialized = false;
        private bool predictionsFinalized = false;

        int width;
        int height;

        private void Awake()
        {
            mouseEventSource.ClickedAt += OnBoardClicked;
        }

        private void OnDestroy()
        {
            mouseEventSource.ClickedAt -= OnBoardClicked;
        }

        public void Initialize(Tile[,] initialGrid)
        {
            grid = initialGrid;

            width = initialGrid.GetLength(0);
            height = initialGrid.GetLength(1);

            initialized = true;
            predictionsFinalized = false;
        }

        public Prediction GetPredictions()
        {
            predictionsFinalized = true;

            var predictions = new Prediction();

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    if (grid[x, y].Marked)
                        predictions.PredictedTiles.Add(grid[x, y]);
                }

            gameObject.SetActive(false);

            return predictions;
        }

        private void OnBoardClicked(Vector2 clickPos)
        {
            if (!initialized || predictionsFinalized)
                return;

            Vector2Int? gridPosOrNull = boardRenderer.PixelSpaceToGridCoordinates(clickPos);

            if (gridPosOrNull is null)
                return;

            var gridPos = (Vector2Int)gridPosOrNull;

            TogglePrediction(gridPos);
        }

        private void TogglePrediction(Vector2Int pos)
        {
            Tile tile = grid[pos.x, pos.y];

            if (tile.IsStone)
                return;

            tile.Marked = !tile.Marked;

            boardRenderer.UpdatePrediction(tile);
        }
    }
}