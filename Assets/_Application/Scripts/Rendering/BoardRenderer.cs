using GMTK2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Rendering
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private TileData tileData = null;

        private Dictionary<Tile, TileRenderer> tileDictionary = new Dictionary<Tile, TileRenderer>();
        private Tile[,] initialGrid;
        int width;
        int height;

        public void RenderInitial(Tile[,] grid)
        {
            initialGrid = grid;

            width = grid.GetLength(0);
            height = grid.GetLength(1);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = grid[x, y];
                    TileRenderer tileRenderer = Instantiate(tileData.PrefabMap[tile.Color], transform);

                    tileRenderer.transform.localPosition = new Vector3(x, y, 0);
                    tileDictionary[tile] = tileRenderer;
                }
        }

        public void RenderSimulation(Simulation simulation, int correctPredictions)
        {

        }

        public Vector2Int? PixelSpaceToGridCoordinates(Vector3 mousePosition)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector3 localPos = worldPos - transform.position;

            var gridPos = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));

            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width || gridPos.y >= width)
                return null;

            return gridPos;
        }

        public void UpdatePrediction(Vector2Int pos, int value)
        {
            Tile tile = initialGrid[pos.x, pos.y];
            tileDictionary[tile].UpdatePrediction(value);
        }
    } 
}
