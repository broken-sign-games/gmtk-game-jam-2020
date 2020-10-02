using DG.Tweening;
using GMTK2020.Audio;
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
        [SerializeField] private StepRenderer[] stepRenderers = null;
        [SerializeField] private Button retryButton = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private SpriteRenderer border = null;
        [SerializeField] private TileRenderer tileRendererPrefab = null;

        [SerializeField] private float postMatchDelay = 0.25f;
        [SerializeField] private float postFallDelay = 0.1f;
        [SerializeField] private float fallingSpeed = 1f;
        [SerializeField] private Ease fallingEase = Ease.InCubic;

        private SoundManager soundManager;

        public event Action SimulationRenderingCompleted;

        private readonly Dictionary<Tile, TileRenderer> tileDictionary = new Dictionary<Tile, TileRenderer>();
        private Board initialBoard;
        int width;
        int height;

        bool cancelAnimation = false;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();
        }

        public void RenderInitial(Board board)
        {
            if (cancelAnimation)
                return;

            foreach (TileRenderer tileRenderer in tileDictionary.Values)
            {
                if (tileRenderer)
                    Destroy(tileRenderer.gameObject);
            }

            tileDictionary.Clear();

            initialBoard = board;

            width = board.Width;
            height = board.Height;

            border.size = new Vector2(width + 0.375f, height + 0.375f);
            transform.localPosition = new Vector2(-(width - 1) / 2f, -(height - 1) / 2f);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = board[x, y];
                    if (tile is null)
                        continue;

                    TileRenderer tileRenderer = Instantiate(tileRendererPrefab, transform);

                    tileRenderer.SetTile(tile);
                    tileDictionary[tile] = tileRenderer;
                }
        }

        public async void KickOffRenderSimulation(Simulation simulation, LevelResult result)
        {
            await RenderSimulationAsync(simulation, result);
        }

        public async Task RenderSimulationAsync(Simulation simulation, LevelResult levelResult)
        {
            await new WaitForSeconds(postMatchDelay * 2);

            for (int i = 0; i < simulation.Steps.Count; ++i)
            {
                if (i > 0)
                    await new WaitForSeconds(postFallDelay);

                if (cancelAnimation)
                    return;

                SimulationStep step = simulation.Steps[i];
                Sequence seq = DOTween.Sequence();

                bool incorrectStep = false;

                if (stepRenderers.Length > i)
                {
                    if (i < levelResult.CorrectPredictions)
                    {
                        stepRenderers[i].ShowSuccess(i);
                    }
                    else
                    {
                        incorrectStep = true;
                        stepRenderers[i].ShowFailure();
                    }
                }

                foreach (Tile tile in step.MatchedTiles)
                {
                    // TODO: indicate incorrect guesses
                    TileRenderer tileRenderer = tileDictionary[tile];

                    bool missedPrediction = incorrectStep && levelResult.MissingPredictions.Contains(tile);
                    if (missedPrediction)
                        tileRenderer.ShowMissingPrediction();

                    seq.Insert(0, tileRenderer.ShowCorrectPrediction());

                    if (!missedPrediction)
                        seq.AppendCallback(() => Destroy(tileRenderer.gameObject));
                }

                if (incorrectStep)
                {
                    foreach (Tile tile in levelResult.ExtraneousPredictions)
                        tileDictionary[tile].ShowIncorrectPrediction();
                }

                await CompletionOf(seq);

                await new WaitForSeconds(postMatchDelay);

                if (cancelAnimation)
                    return;

                if (i >= levelResult.CorrectPredictions)
                    break;

                seq = DOTween.Sequence();

                foreach (Tile tile in step.MovingTiles)
                {
                    TileRenderer tileRenderer = tileDictionary[tile];
                    Tween tween = tileRenderer.transform
                        .DOLocalMove(new Vector3Int(tile.Position.x, tile.Position.y, 0), fallingSpeed)
                        .SetSpeedBased()
                        .SetEase(fallingEase);
                    seq.Join(tween);
                }

                await CompletionOf(seq);

                if (cancelAnimation)
                    return;
            }

            if (retryButton && nextButton)
            {
                await new WaitForSeconds(postMatchDelay);

                if (levelResult.CorrectPredictions < Simulator.MAX_SIMULATION_STEPS)
                {
                    retryButton.gameObject.SetActive(true);
                }
                else
                {
                    if (soundManager)
                        soundManager.PlayEffect(SoundManager.Effect.WIN);
                    nextButton.gameObject.SetActive(true);
                }
            }

            SimulationRenderingCompleted?.Invoke();
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

        public void UpdatePrediction(Vector2Int pos)
        {
            Tile tile = initialBoard[pos];
            UpdatePrediction(tile);
        }

        public void UpdatePrediction(Tile tile)
        {
            tileDictionary[tile].UpdatePrediction();

            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.PREDICT);
        }
    }
}
