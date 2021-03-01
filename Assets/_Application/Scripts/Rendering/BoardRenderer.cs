using DG.Tweening;
using GMTK2020.Data;
using GMTK2020.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GMTK2020.Rendering
{
    public class BoardRenderer : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private TileRenderer tileRendererPrefab = null;
        [SerializeField] private ChainCounter chainCounter = null;

        [SerializeField] private float postMatchDelay = 0.25f;
        [SerializeField] private float postFallDelay = 0.1f;
        [SerializeField] private float postInertDelay = 0.1f;
        [SerializeField] private float spikeBallDelay = 0.25f;
        [SerializeField] private float anticipateSpikeBallDestructionBy = 0.25f;

        private readonly Dictionary<Guid, TileRenderer> tileDictionary = new Dictionary<Guid, TileRenderer>();
        
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

            width = board.Width;
            height = board.Height;

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

        public async Task AnimateNewTurn()
        {
            await CompletionOf(chainCounter.ResetChain());
        }

        public async Task AnimateSimulationStepAsync(SimulationStep step)
        {
            switch (step)
            {
            case MatchStep matchStep: await AnimateMatchStepAsync(matchStep); break;
            case CleanUpStep cleanUpStep: await AnimateCleanUpStepAsync(cleanUpStep); break;
            case RemovalStep removalStep: await AnimateRemovalStepAsync(removalStep); break;
            case PermutationStep permutationStep: await AnimatePermutationStepAsync(permutationStep); break;
            case RotationStep rotationStep: await AnimateRotationStepAsync(rotationStep); break;
            case PredictionStep predictionStep: await AnimatePredictionStepAsync(predictionStep); break;
            case RefillStep refillStep: await AnimateRefillStepAsync(refillStep); break;
            case WildcardStep wildcardStep: await AnimateWildcardStepAsync(wildcardStep); break;
            }
        }

        private async Task AnimateMatchStepAsync(MatchStep step)
        {
            await AnimateMatchedTilesAsync(step.MatchedTiles);
            await AnimateFallingTilesAsync(step.MovedTiles);
        }

        private async Task AnimateCleanUpStepAsync(CleanUpStep step)
        {
            await AnimateInertTilesAsync(step.InertTiles);
            await AnimateCrackedTilesAsync(step.CrackedTiles);
            await AnimateNewTilesAsync(step.NewTiles);
        }

        private async Task AnimateRemovalStepAsync(RemovalStep step)
        {
            await AnimateRemovedTilesAsync(step.RemovedTiles);
            await AnimateNewAndFallingTilesAsync(step.NewTiles, step.MovedTiles);
        }

        private async Task AnimatePermutationStepAsync(PermutationStep step)
        {
            await AnimateMovingTilesAsync(step.MovedTiles);
        }

        private async Task AnimateRotationStepAsync(RotationStep step)
        {
            await AnimateRotatingTilesAsync(step.MovedTiles, step.Pivot, step.RotationSense);
        }

        private async Task AnimatePredictionStepAsync(PredictionStep predictionStep)
        {
            Sequence seq = DOTween.Sequence();
            foreach (Tile tile in predictionStep.AffectedTiles)
                seq.Join(UpdatePrediction(tile));

            await CompletionOf(seq);
        }

        private async Task AnimateRefillStepAsync(RefillStep refillStep)
        {
            Sequence seq = DOTween.Sequence();
            foreach (Tile tile in refillStep.AffectedTiles)
                seq.Join(RefillTile(tile));

            await CompletionOf(seq);
        }

        private async Task AnimateWildcardStepAsync(WildcardStep wildcardStep)
        {
            Sequence seq = DOTween.Sequence();
            foreach (Tile tile in wildcardStep.AffectedTiles)
                seq.Join(MakeWildcard(tile));

            await CompletionOf(seq);
        }

        private async Task AnimateMatchedTilesAsync(HashSet<Tile> matchedTiles)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(chainCounter.AddChain());

            foreach (Tile tile in matchedTiles)
            {
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.MatchAndDestroy());

                tileDictionary.Remove(tile.ID);
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postMatchDelay);
        }

        private async Task AnimateFallingTilesAsync(List<MovedTile> movedTiles)
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

        private async Task AnimateRemovedTilesAsync(HashSet<Tile> removedTiles)
        {
            Sequence seq = DOTween.Sequence();

            foreach (Tile tile in removedTiles)
            {
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.Destroy());

                tileDictionary.Remove(tile.ID);
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postMatchDelay);
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

        private async Task AnimateCrackedTilesAsync(HashSet<Tile> crackedTiles)
        {
            Sequence seq = DOTween.Sequence();

            float delay = 0f;

            foreach (Tile tile in crackedTiles)
            {
                if (tile.Cracks == 1)
                    continue;

                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(delay, tileRenderer.UpdateCracks());

                delay += spikeBallDelay;
            }

            foreach (Tile tile in crackedTiles)
            {
                if (tile.Cracks > 1)
                    continue;

                TileRenderer tileRenderer = tileDictionary[tile.ID];

                Tween spikeBallTween = chainCounter.SendSpikeBall(tileRenderer.transform.position);
                seq.Insert(delay, spikeBallTween);

                seq.Insert(delay + spikeBallTween.Duration() - anticipateSpikeBallDestructionBy, tileRenderer.UpdateCracks());

                delay += spikeBallDelay;
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

        private async Task AnimateNewAndFallingTilesAsync(List<MovedTile> newTiles, List<MovedTile> movedTiles)
        {
            Sequence seq = DOTween.Sequence();

            foreach (MovedTile movedTile in newTiles)
            {
                TileRenderer tileRenderer = Instantiate(tileRendererPrefab, transform);

                tileRenderer.SetTile(movedTile.Tile, movedTile.From);
                tileDictionary[movedTile.Tile.ID] = tileRenderer;

                seq.Insert(0, tileRenderer.FallToCurrentPosition(movedTile.From));
            }

            foreach (MovedTile movedTile in movedTiles)
            {
                Tile tile = movedTile.Tile;
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.FallToCurrentPosition(movedTile.From));
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postFallDelay);
        }

        private async Task AnimateMovingTilesAsync(List<MovedTile> movedTiles)
        {
            Sequence seq = DOTween.Sequence();

            foreach (MovedTile movedTile in movedTiles)
            {
                Tile tile = movedTile.Tile;
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.MoveToCurrentPosition(movedTile.From));
            }

            await CompletionOf(seq);

            await new WaitForSeconds(postFallDelay);
        }

        private async Task AnimateRotatingTilesAsync(List<MovedTile> movedTiles, Vector2 pivot, RotationSense rotSense)
        {
            Sequence seq = DOTween.Sequence();

            foreach (MovedTile movedTile in movedTiles)
            {
                Tile tile = movedTile.Tile;
                TileRenderer tileRenderer = tileDictionary[tile.ID];

                seq.Insert(0, tileRenderer.RotateToCurrentPosition(movedTile.From, pivot, rotSense));
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

        public Vector2Int? PixelSpaceToHalfGridCoordinates(Vector2 mousePosition)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector3 localPos = worldPos - transform.position;

            var gridPos = new Vector2Int(Mathf.RoundToInt(localPos.x - 0.5f), Mathf.RoundToInt(localPos.y - 0.5f));

            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= width - 1 || gridPos.y >= height - 1)
                return null;

            return gridPos;
        }

        private Tween UpdatePrediction(Tile tile)
        {
            return tileDictionary[tile.ID].UpdatePrediction();
        }

        private Tween RefillTile(Tile tile)
        {
            return tileDictionary[tile.ID].Refill();
        }

        private Tween MakeWildcard(Tile tile)
        {
            return tileDictionary[tile.ID].MakeWildcard();
        }
    }
}
