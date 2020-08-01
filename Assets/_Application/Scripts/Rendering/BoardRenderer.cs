using DG.Tweening;
using GMTK2020.Data;
using GMTK2020.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.Rendering
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private TileData tileData = null;
        [SerializeField] private SpriteRenderer border = null;
        [SerializeField] private Transform clearedRowRoot = null;
        [SerializeField] private ClearedRowRenderer clearedRowPrefab = null;
        [SerializeField] private ScoreDisplay scoreDisplay = null;

        [SerializeField] private float postMatchDelay = 0.25f;
        [SerializeField] private float postFallDelay = 0.1f;
        [SerializeField] private float fallingSpeed = 1f;
        [SerializeField] private Ease fallingEase = Ease.InCubic;

        private SoundManager soundManager = null;

        public event Action<bool> SimulationRenderingCompleted;

        private Dictionary<Tile, TileRenderer> tileDictionary = new Dictionary<Tile, TileRenderer>();
        private List<ClearedRowRenderer> clearedRowRenderers = new List<ClearedRowRenderer>();
        int width;
        int height;

        bool cancelAnimation = false;

        private ScoreKeeper scoreKeeper;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();
        }

        public void RenderInitial(Tile[,] grid, ScoreKeeper scoreKeeper)
        {
            if (cancelAnimation)
                return;

            foreach (TileRenderer tileRenderer in tileDictionary.Values)
            {
                if (tileRenderer)
                    Destroy(tileRenderer.gameObject);
            }

            tileDictionary.Clear();

            width = grid.GetLength(0);
            height = grid.GetLength(1);

            border.size = new Vector2(width + 0.375f, height + 0.375f);
            transform.localPosition = new Vector2(-(width - 1) / 2f, -(height - 1) / 2f);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = grid[x, y];
                    if (tile is null)
                        continue;

                    TileRenderer tileRenderer = Instantiate(tileData.PrefabMap[tile.Color], transform);

                    tileRenderer.transform.localPosition = new Vector3(x, y, 0);
                    tileDictionary[tile] = tileRenderer;
                }

            this.scoreKeeper = scoreKeeper;

            scoreDisplay.SetScore(0);
            scoreDisplay.SetHighscore(scoreKeeper.Highscore);
        }

        public async void KickOffRenderSimulation(Simulation simulation)
        {
            await RenderSimulationAsync(simulation);
        }

        public async Task RenderSimulationAsync(Simulation simulation)
        {
            await new WaitForSeconds(postMatchDelay * 2);

            Sequence seq;

            for (int i = 0; i < simulation.Steps.Count; ++i)
            {
                if (i > 0)
                    await new WaitForSeconds(postFallDelay);

                if (cancelAnimation)
                    return;

                SimulationStep step = simulation.Steps[i];
                seq = DOTween.Sequence();

                if (i > 0)
                {
                    ClearedRowRenderer rowRenderer = Instantiate(clearedRowPrefab, clearedRowRoot);
                    rowRenderer.transform.localPosition = new Vector3(0, i - 1, 0);

                    clearedRowRenderers.Add(rowRenderer);

                    seq.Append(rowRenderer.ShowIndicator());
                }

                foreach ((Tile tile, _) in step.MatchedTiles)
                {
                    TileRenderer tileRenderer = tileDictionary[tile];

                    seq.Insert(0, tileRenderer.ShowCorrectPrediction());

                    Tile capturedTile = tile;

                    seq.AppendCallback(() => {
                        tileDictionary.Remove(capturedTile);
                        Destroy(tileRenderer.gameObject);
                    });
                }

                await CompletionOf(seq);

                scoreDisplay.SetScore(step.Score);

                await new WaitForSeconds(postMatchDelay);

                if (cancelAnimation)
                    return;

                foreach ((Tile tile, Vector2Int newPosition) in step.NewTiles)
                {
                    TileRenderer tileRenderer = Instantiate(tileData.PrefabMap[tile.Color], transform);

                    tileRenderer.transform.localPosition = new Vector3(newPosition.x, newPosition.y, 0);
                    tileDictionary[tile] = tileRenderer;
                }

                seq = DOTween.Sequence();

                foreach ((Tile tile, Vector2Int newPosition) in step.MovingTiles)
                {
                    TileRenderer tileRenderer = tileDictionary[tile];
                    Tween tween = tileRenderer.transform
                        .DOLocalMove(new Vector3Int(newPosition.x, newPosition.y, 0), 0.75f)
                        .SetSpeedBased()
                        .SetEase(fallingEase);
                    seq.Join(tween);
                }

                await CompletionOf(seq);

                if (cancelAnimation)
                    return;
            }

            seq = DOTween.Sequence();

            foreach (Tile tile in simulation.ClearBoardStep.ExtraneousPredictions)
            {
                TileRenderer tileRenderer = tileDictionary[tile];

                seq.Insert(0, tileRenderer.Petrify());
            }

            await CompletionOf(seq);

            seq = DOTween.Sequence();

            foreach (ClearedRowRenderer rowRenderer in clearedRowRenderers)
            {
                ClearedRowRenderer capturedRenderer = rowRenderer;

                seq.Insert(0, rowRenderer.ClearRow());
                seq.AppendCallback(() => Destroy(rowRenderer.gameObject));
            }

            clearedRowRenderers.Clear();

            foreach ((Tile tile, _) in simulation.ClearBoardStep.ClearedTiles)
            {
                TileRenderer tileRenderer = tileDictionary[tile];

                seq.Insert(0, tileRenderer.Explode());

                Tile capturedTile = tile;

                seq.AppendCallback(() => {
                    tileDictionary.Remove(capturedTile);
                    Destroy(tileRenderer.gameObject);
                });
            }

            await CompletionOf(seq);

            foreach ((Tile tile, Vector2Int newPosition) in simulation.ClearBoardStep.NewTiles)
            {
                TileRenderer tileRenderer = Instantiate(tileData.PrefabMap[tile.Color], transform);

                tileRenderer.transform.localPosition = new Vector3(newPosition.x, newPosition.y, 0);
                tileDictionary[tile] = tileRenderer;
            }

            seq = DOTween.Sequence();

            foreach ((Tile tile, Vector2Int newPosition) in simulation.ClearBoardStep.MovingTiles)
            {
                TileRenderer tileRenderer = tileDictionary[tile];
                Tween tween = tileRenderer.transform
                    .DOLocalMove(new Vector3Int(newPosition.x, newPosition.y, 0), 0.75f)
                    .SetSpeedBased()
                    .SetEase(fallingEase);
                seq.Join(tween);
            }

            await CompletionOf(seq);

            if (!simulation.FurtherMatchesPossible && scoreKeeper.Score > scoreKeeper.Highscore)
                scoreDisplay.SetHighscore(scoreKeeper.Score);

            SimulationRenderingCompleted?.Invoke(simulation.FurtherMatchesPossible);
        }

        System.Collections.IEnumerator CompletionOf(Tween tween)
        {
            yield return tween.WaitForCompletion();
        }

        public void CancelAnimation()
        {
            cancelAnimation = true;
        }

        public Vector2Int? PixelSpaceToGridCoordinates(Vector3 mousePosition)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector3 localPos = worldPos - transform.position;

            var gridPos = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));

            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width || gridPos.y >= height)
                return null;

            return gridPos;
        }

        public void UpdatePrediction(Tile tile, bool isPredicted)
        {
            tileDictionary[tile].UpdatePrediction(isPredicted);
            if (soundManager != null)
                soundManager.PlayEffect(SoundManager.Effect.PREDICT, 1);
        }
    }
}
