using DG.Tweening;
using GMTK2020.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async void KickOffRenderSimulation(Simulation simulation, int correctPredictions)
        {
            await RenderSimulationAsync(simulation, correctPredictions);
        }

        System.Collections.IEnumerator FoobarAsync(Tween tween)
        {
            yield return tween.WaitForCompletion();
        }

        public async Task RenderSimulationAsync(Simulation simulation, int correctPredictions)
        {
            var rnd = new System.Random();
            foreach (var step in simulation.Steps)
            {
                List<Tween> tweens = new List<Tween>();

                foreach (var tile in step.MatchedTiles)
                {
                    var tileRenderer = tileDictionary[tile];
                    tweens.Add(tileRenderer.transform.DOLocalMove(new Vector3Int(rnd.Next(30, 40), rnd.Next(-10, 12), 0), 1.0f));
                }

                foreach (var (tile, newPosition) in step.MovingTiles)
                {
                    var tileRenderer = tileDictionary[tile];
                    tweens.Add(tileRenderer.transform.DOLocalMove(new Vector3Int(newPosition.x, newPosition.y, 0), 1.0f));
                }

                // omg why can't I wait on all of them more easily?
                foreach (var tween in tweens)
                {
                    await FoobarAsync(tween);
                }
            }

            // iterate over steps
            //   Destroy(tileRenderer.gameObject);
            //   animate disappearing tiles + checkmark/cross
            //   TODO: indicate incorrect guesses
            //   wait for completion
            //   tween falling tiles
            //   wait for completion
            // if correct solution:
            //   show "next level" button
            // else
            //   show "retry level" button
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
