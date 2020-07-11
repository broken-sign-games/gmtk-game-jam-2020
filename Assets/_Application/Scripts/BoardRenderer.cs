using GMTK2020.Data;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private TileData tileData;

        private Dictionary<Tile, SpriteRenderer> tileDictionary = new Dictionary<Tile, SpriteRenderer>();

        public void RenderInitial(Tile[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = grid[x, y];
                    SpriteRenderer tileRenderer = Instantiate(tileData.PrefabMap[tile.Color], transform);

                    tileRenderer.transform.localPosition = new Vector3(x, y, 0);
                    tileDictionary[tile] = tileRenderer;
                }
        }

        public void RenderSimulation(Simulation simulation, int correctPredictions)
        {

        }
    } 
}
