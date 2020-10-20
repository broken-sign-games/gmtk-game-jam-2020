using DG.Tweening;
using GMTK2020.Audio;
using GMTK2020.Data;
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
        [SerializeField] private Button retryButton = null;
        [SerializeField] private Button nextButton = null;
        [SerializeField] private SpriteRenderer border = null;
        [SerializeField] private SpriteRenderer gridLines = null;
        [SerializeField] private TileRenderer tileRendererPrefab = null;

        [SerializeField] private float postMatchDelay = 0.25f;
        [SerializeField] private float postFallDelay = 0.1f;
        [SerializeField] private float postInertDelay = 0.1f;

        private readonly Dictionary<Guid, TileRenderer> tileDictionary = new Dictionary<Guid, TileRenderer>();
        private Board initialBoard;

        int width;
        int height;

        bool cancelAnimation = false;

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

            border.size = new Vector2(width + 0.375f - 0.03125f, height + 0.375f - 0.03125f);
            gridLines.size = new Vector2(width - 0.03125f, height - 0.03125f);
            transform.localPosition = new Vector2(-(width - 1) / 2f, -(height - 1) / 2f);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = board[x, y];
                    if (tile is null)
                        continue;

                    TileRenderer tileRenderer = Instantiate(tileRendererPrefab, transform);

                    tileRenderer.SetTile(tile);
                    tileDictionary[tile.ID] = tileRenderer;
                }
        }
        public async Task AnimateSimulationStepAsync(SimulationStep step)
        {
            switch (step)
            {
            case MatchStep matchStep: await AnimateMatchStepAsync(matchStep); break;
            case CleanUpStep cleanUpStep: await RenderCleanUpStepAsync(cleanUpStep); break;
            }
        }

        private async Task AnimateMatchStepAsync(MatchStep step)
        {
            await AnimateMatchedTilesAsync(step.MatchedTiles);
            await AnimateMovingTilesAsync(step.MovedTiles);
        }

        private async Task RenderCleanUpStepAsync(CleanUpStep step)
        {
            await AnimateInertTilesAsync(step.InertTiles);
            await AnimateNewTilesAsync(step.NewTiles);
        }

        private async Task AnimateMatchedTilesAsync(HashSet<Tile> matchedTiles)
        {
            Sequence seq = DOTween.Sequence();

            foreach (Tile tile in matchedTiles)
            {
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.MatchAndDestroy());

                tileDictionary.Remove(tile.ID);
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postMatchDelay);
        }

        private async Task AnimateMovingTilesAsync(List<MovedTile> movedTiles)
        { 
            Sequence seq = DOTween.Sequence();

            foreach (MovedTile movedTile in movedTiles)
            {
                Tile tile = movedTile.Tile;
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.FallToCurrentPosition(movedTile.From));
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postFallDelay);
        }

        private async Task AnimateInertTilesAsync(HashSet<Tile> inertTiles)
        {
            Sequence seq = DOTween.Sequence();

            foreach (Tile tile in inertTiles)
            {
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.TransitionToInert());
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postInertDelay);
        }

        private async Task AnimateNewTilesAsync(List<MovedTile> newTiles)
        {
            Sequence seq = DOTween.Sequence();

            foreach (MovedTile movedTile in newTiles)
            {
                TileRenderer tileRenderer = Instantiate(tileRendererPrefab, transform);

                tileRenderer.SetTile(movedTile.Tile, movedTile.From);
                tileDictionary[movedTile.Tile.ID] = tileRenderer;

                seq.Insert(0, tileRenderer.FallToCurrentPosition(movedTile.From));
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postFallDelay);
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
            tileDictionary[tile.ID].UpdatePrediction();
        }
    }
}
