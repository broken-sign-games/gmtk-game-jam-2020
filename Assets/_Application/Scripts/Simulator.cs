using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public class Simulator
    {
        public const int MAX_SIMULATION_STEPS = 5;

        public event Action ReactionUnlocked;
        public event Action ReactionLocked;
        private bool isReactionAllowed = false;

        private readonly Board board;

        private readonly Random rng;
        private int colorCount;
        private readonly LevelSpecification levelSpec;

        private enum DifficultyIncrease
        {
            AddColor,
            AddCrack,
        }

        public int DifficultyLevel { get; private set; } = 0;
        private DifficultyIncrease nextDifficultyIncrease = DifficultyIncrease.AddCrack;
        private int remainingResource;
        private int resourceCost;
        
        
        private bool ensureLatestColorDrops = false;

        private int turnCount = 0;
        public int ChainLength { get; private set; } = 0;

        private readonly Queue<Tile> fixedCrackQueue = new Queue<Tile>();

        private List<Tile> tutorialTilesToOpen = null;

        public Simulator(Board initialBoard, LevelSpecification levelSpec)
        {
            board = initialBoard;
            this.levelSpec = levelSpec;

            colorCount = levelSpec.InitialColorCount;

            remainingResource = levelSpec.InitialResource;
            resourceCost = levelSpec.InitialCracksPerChain;

            if (TutorialManager.Instance)
                TutorialManager.Instance.TutorialReady += OnTutorialReady;

            // TODO: We probably want more control over the seed...
            rng = new Random(Time.frameCount);
        }

        public void SetFixedCracks(Vector2Int[] crackedTiles)
        {
            foreach (Vector2Int pos in crackedTiles)
            {
                Tile tile = board[pos];
                fixedCrackQueue.Enqueue(tile);
            }
        }

        public SimulationStep SimulateNextStep()
        {
            List<IEnumerable<Tile>> matches = GetRawMatches();

            (HashSet<Vector2Int> horizontalMatches, HashSet<Vector2Int> verticalMatches) = GetMatchPositions(matches);

            HashSet<Tile> matchedTiles = RemoveMatchedTiles(matches);

            if (matchedTiles.Count > 0)
            {
                List<MovedTile> movedTiles = MoveTilesDown();

                ++ChainLength;
                ++remainingResource;

                return new MatchStep(ChainLength, 1, matchedTiles, movedTiles, horizontalMatches, verticalMatches);
            }

            ++turnCount;

            HashSet<Tile> inertTiles = MakeMarkedAndCrackedTilesInert();

            int previousResourceCost = resourceCost;

            IncreaseDifficulty();

            List<MovedTile> newTiles = FillBoardWithTiles();

            ChainLength = 0;
            remainingResource -= previousResourceCost;

            return new CleanUpStep(newTiles, inertTiles, previousResourceCost);
        }

        private Task OnTutorialReady(Tutorial tutorial)
        {
            int rectCount = tutorial.InteractableRects?.Count ?? 0;

            if (rectCount == 0)
                return Task.CompletedTask;

            if (tutorial.InteractableTools.Count == 0)
                RegisterTilesToOpenForTutorial(tutorial.InteractableRects);

            return Task.CompletedTask;
        }

        private void RegisterTilesToOpenForTutorial(List<GridRect> interactableRects)
        {
            tutorialTilesToOpen = new List<Tile>();

            foreach (GridRect rect in interactableRects)
                foreach (Vector2Int pos in rect.GetPositions())
                    tutorialTilesToOpen.Add(board[pos]);
        }

        private List<IEnumerable<Tile>> GetRawMatches()
        {
            var matches = new List<IEnumerable<Tile>>();

            foreach (IEnumerable<Tile> tiles in GetPotentialMatches())
                if (IsMatch(tiles))
                    matches.Add(tiles);

            return matches;
        }

        private (HashSet<Vector2Int> horizontalMatches, HashSet<Vector2Int> verticalMatches) GetMatchPositions(IEnumerable<IEnumerable<Tile>> matches)
        {
            var horizontalMatches = new HashSet<Vector2Int>();
            var verticalMatches = new HashSet<Vector2Int>();

            foreach (IEnumerable<Tile> tiles in matches)
            {
                Vector2Int startOfMatch = tiles.First().Position;
                if (IsHorizontal(tiles))
                    horizontalMatches.Add(startOfMatch);
                else
                    verticalMatches.Add(startOfMatch);
            }

            return (horizontalMatches, verticalMatches);
        }

        public List<SimulationStep> SimulateToStop()
        {
            var steps = new List<SimulationStep>();

            do
            {
                steps.Add(SimulateNextStep());
            }
            while (!steps.Last().FinalStep);

            return steps;
        }

        private HashSet<Tile> RemoveMatchedTiles(List<IEnumerable<Tile>> matches)
        {
            var matchedTiles = new HashSet<Tile>();

            foreach (IEnumerable<Tile> tiles in matches)
                foreach (Tile tile in tiles)
                    matchedTiles.Add(tile);

            foreach (Tile tile in matchedTiles)
                board[tile.Position] = null;

            return matchedTiles;
        }

        public bool AnyResourceLeft()
            => remainingResource > 0;

        public bool FurtherMatchesPossible()
        {
            return GetPotentialMatches().Any(tiles => IsMatch(tiles, false));
        }

        private List<IEnumerable<Tile>> GetPotentialMatches()
        {
            var potentialMatches = new List<IEnumerable<Tile>>();

            foreach (int x in board.GetXs())
            {
                foreach (int yMax in board.GetYs().Skip(2))
                {
                    potentialMatches.Add(Enumerable
                        .Range(yMax - 2, 3)
                        .Select(y => board[x, y]));
                }
            }


            foreach (int y in board.GetYs())
            {
                foreach (int xMax in board.GetXs().Skip(2))
                {
                    potentialMatches.Add(Enumerable
                        .Range(xMax - 2, 3)
                        .Select(x => board[x, y]));
                }
            }

            return potentialMatches;
        }

        private bool IsMatch(IEnumerable<Tile> tiles, bool requiredMarked = true)
        {
            if (tiles.Any(tile => tile is null || tile.Inert || requiredMarked && !tile.Marked))
                return false;

            int nDistinctColors = tiles
                .Select(tile => tile.Wildcard ? -1 : tile.Color)
                .Distinct()
                .Where(color => color != -1)
                .Count();

            return nDistinctColors <= 1;
        }

        private bool IsHorizontal(IEnumerable<Tile> tiles)
            => tiles.Select(t => t.Position.y).Distinct().Count() == 1;

        public List<MovedTile> MoveTilesDown()
        {
            var movedTiles = new List<MovedTile>();

            foreach (int x in board.GetXs())
            {
                int top = 0;
                foreach (int y in board.GetYs(VerticalOrder.BottomToTop))
                {
                    Vector2Int from = new Vector2Int(x, y);
                    Tile tile = board[from];
                    if (tile is null)
                        continue;

                    if (y > top)
                    {
                        movedTiles.Add(
                            board.MoveTile(tile, x, top)
                        );
                    }

                    ++top;
                }
            }

            return movedTiles;
        }

        private HashSet<Tile> MakeMarkedAndCrackedTilesInert()
        {
            var inertTiles = new HashSet<Tile>();

            foreach (Tile tile in board)
            {
                if (!tile.Inert && tile.Marked)
                {
                    tile.MakeInert();
                    inertTiles.Add(tile);
                }
            }

            return inertTiles;
        }

        private void IncreaseDifficulty()
        {
            if (turnCount % levelSpec.ChainsPerDifficultyIncrease != 0)
                return;

            ++DifficultyLevel;
            
            switch (nextDifficultyIncrease)
            {
            case DifficultyIncrease.AddColor:
                if (colorCount < levelSpec.MaxColorCount)
                {
                    ++colorCount;
                    ensureLatestColorDrops = true;
                }
                nextDifficultyIncrease = DifficultyIncrease.AddCrack;
                break;
            case DifficultyIncrease.AddCrack:
                // TODO: This will increase resource cost instead
                if (resourceCost < levelSpec.MaxCracksPerChain)
                    ++resourceCost;
                nextDifficultyIncrease = DifficultyIncrease.AddColor;
                break;
            default:
                break;
            }
        }

        private List<MovedTile> FillBoardWithTiles()
        {
            var newTiles = new List<MovedTile>();

            int nNewTiles = board.Width * board.Height - board.Count();
            int newColorIndex = rng.Next(nNewTiles);

            int i = 0;
            foreach (int x in board.GetXs())
            {
                int newTilesInColumn = board.Height;
                foreach (int y in board.GetYs(VerticalOrder.BottomToTop))
                {
                    if (board[x, y] != null)
                    {
                        --newTilesInColumn;
                        continue;
                    }

                    int color = (ensureLatestColorDrops && i == newColorIndex)
                        ? colorCount - 1
                        : rng.Next(colorCount);

                    Tile newTile = new Tile(color, new Vector2Int(x, y + newTilesInColumn));
                    newTiles.Add(board.MoveTile(newTile, x, y));

                    ++i;
                }
            }

            ensureLatestColorDrops = false;

            return newTiles;
        }

        public RemovalStep RemoveTile(Vector2Int pos)
        {
            var positions = new[] { pos };

            return RemoveTiles(positions);
        }

        public RemovalStep RemoveBlock(Vector2Int center)
        {
            var positions = new List<Vector2Int>();

            for (int y = -1; y <= 1; ++y)
                for (int x = -1; x <= 1; ++x)
                {
                    Vector2Int pos = center + new Vector2Int(x, y);
                    if (board.IsInBounds(pos))
                        positions.Add(pos);
                }

            return RemoveTiles(positions);
        }

        public RemovalStep RemovePlus(Vector2Int center)
        {
            IEnumerable<Vector2Int> positions = new[]
            {
                center,
                center + Vector2Int.right,
                center + Vector2Int.up,
                center + Vector2Int.left,
                center + Vector2Int.down,
            }.Where(pos => board.IsInBounds(pos));

            return RemoveTiles(positions);
        }

        public RemovalStep RemoveRow(int y)
        {
            IEnumerable<Vector2Int> positions = board.GetXs().Select(x => new Vector2Int(x, y));

            return RemoveTiles(positions);
        }

        public RemovalStep RemoveColor(Vector2Int gridPos)
            => RemoveColor(board[gridPos].Color);

        public RemovalStep RemoveColor(int color)
        {
            List<Vector2Int> positions = board
                .Where(t => t.Color == color)
                .Select(t => t.Position)
                .ToList();

            return RemoveTiles(positions);
        }

        private RemovalStep RemoveTiles(IEnumerable<Vector2Int> positions)
        {
            var removedTiles = new HashSet<Tile>();

            foreach (Vector2Int pos in positions)
            {
                removedTiles.Add(board[pos]);
                board[pos] = null;
            }

            List<MovedTile> movedTiles = MoveTilesDown();
            List<MovedTile> newTiles = FillBoardWithTiles();

            TutorialManager.Instance?.CompleteActiveTutorial();

            CheckWhetherReactionIsAllowed();

            return new RemovalStep(removedTiles, movedTiles, newTiles);
        }

        public RefillStep RefillTile(Vector2Int pos)
        {
            Tile tile = board[pos];

            if (!tile.Inert)
                throw new InvalidOperationException("Cannot refill full tile.");

            tile.Refill();

            CheckWhetherReactionIsAllowed();

            return new RefillStep(new List<Tile> { tile });
        }

        public PermutationStep ShuffleBoard()
        {
            var movedTiles = new List<MovedTile>();

            List<Tile> shuffledTiles = board.ToList().Shuffle(rng);

            int i = 0;

            foreach (int y in board.GetYs())
                foreach (int x in board.GetXs())
                {
                    movedTiles.Add(board.MoveTile(shuffledTiles[i], x, y));
                    ++i;
                }

            CheckWhetherReactionIsAllowed();

            return new PermutationStep(movedTiles);
        }

        public RotationStep RotateBoard(RotationSense rotSense)
        {
            var movedTiles = new List<MovedTile>();

            List<Tile> tiles = board.GetRows(HorizontalOrder.LeftToRight, VerticalOrder.BottomToTop).SelectMany(x => x).ToList();

            HorizontalOrder horizontalOrder = rotSense == RotationSense.CW 
                ? HorizontalOrder.LeftToRight
                : HorizontalOrder.RightToLeft;

            VerticalOrder verticalOrder = rotSense == RotationSense.CW
                ? VerticalOrder.TopToBottom
                : VerticalOrder.BottomToTop;

            int i = 0;
            foreach (int x in board.GetXs(horizontalOrder))
                foreach (int y in board.GetYs(verticalOrder))
                {
                    Tile tile = tiles[i];
                    if (tile.Position != new Vector2Int(x, y))
                        movedTiles.Add(board.MoveTile(tile, x, y));

                    ++i;
                }

            CheckWhetherReactionIsAllowed();

            Vector2 pivot = new Vector2(board.Width - 1, board.Height - 1) / 2f;
            return new RotationStep(pivot, rotSense, movedTiles);
        }

        public RotationStep Rotate2x2Block(Vector2Int bottomLeft, RotationSense rotSense)
        {
            if (!board.IsInBounds(bottomLeft) || !board.IsInBounds(bottomLeft + Vector2Int.one))
                throw new InvalidOperationException("2x2 block partially or fully out of bounds");


            var movedTiles = new List<MovedTile>();

            List<Tile> tiles = board.GetRows(HorizontalOrder.LeftToRight, VerticalOrder.BottomToTop)
                .Skip(bottomLeft.y)
                .Take(2)
                .SelectMany(row => row.Skip(bottomLeft.x).Take(2))
                .ToList();

            HorizontalOrder horizontalOrder = rotSense == RotationSense.CW
                ? HorizontalOrder.LeftToRight
                : HorizontalOrder.RightToLeft;

            VerticalOrder verticalOrder = rotSense == RotationSense.CW
                ? VerticalOrder.TopToBottom
                : VerticalOrder.BottomToTop;

            int i = 0;
            foreach (int x in board.GetXs(horizontalOrder))
                foreach (int y in board.GetYs(verticalOrder))
                {
                    if (x < bottomLeft.x || x > bottomLeft.x + 1 || y < bottomLeft.y || y > bottomLeft.y + 1)
                        continue;

                    movedTiles.Add(board.MoveTile(tiles[i], x, y));

                    ++i;
                }

            CheckWhetherReactionIsAllowed();

            Vector2 pivot = bottomLeft + 0.5f * Vector2.one;
            return new RotationStep(pivot, rotSense, movedTiles);
        }

        public RotationStep Rotate3x3Block(Vector2Int pivot, RotationSense rotSense)
        {
            if (!board.IsInBounds(pivot - Vector2Int.one) || !board.IsInBounds(pivot + Vector2Int.one))
                throw new InvalidOperationException("3x3 block partially or fully out of bounds");

            var movedTiles = new List<MovedTile>();

            List<Tile> tiles = board.GetRows(HorizontalOrder.LeftToRight, VerticalOrder.BottomToTop)
                .Skip(pivot.y - 1)
                .Take(3)
                .SelectMany(row => row.Skip(pivot.x - 1).Take(3))
                .ToList();

            HorizontalOrder horizontalOrder = rotSense == RotationSense.CW
                ? HorizontalOrder.LeftToRight
                : HorizontalOrder.RightToLeft;

            VerticalOrder verticalOrder = rotSense == RotationSense.CW
                ? VerticalOrder.TopToBottom
                : VerticalOrder.BottomToTop;

            int i = 0;
            foreach (int x in board.GetXs(horizontalOrder))
                foreach (int y in board.GetYs(verticalOrder))
                {
                    if (x < pivot.x - 1 || x > pivot.x + 1 || y < pivot.y - 1 || y > pivot.y + 1)
                        continue;

                    Tile tile = tiles[i];
                    if (tile.Position != new Vector2Int(x, y))
                        movedTiles.Add(board.MoveTile(tile, x, y));

                    ++i;
                }

            CheckWhetherReactionIsAllowed();

            return new RotationStep(pivot, rotSense, movedTiles);
        }

        public PermutationStep SwapTiles(Vector2Int pos1, Vector2Int pos2)
        {
            Tile tile1 = board[pos1];
            Tile tile2 = board[pos2];

            var movedTiles = new List<MovedTile>
            {
                board.MoveTile(tile1, pos2),
                board.MoveTile(tile2, pos1),
            };

            TutorialManager.Instance?.CompleteActiveTutorial();

            CheckWhetherReactionIsAllowed();

            return new PermutationStep(movedTiles);
        }

        public PermutationStep SwapRows(int y1, int y2)
        {
            List<Tile> row1 = board.GetRow(y1).ToList();
            List<Tile> row2 = board.GetRow(y2).ToList();

            var movedTiles = new List<MovedTile>();

            foreach (Tile tile in row1)
                movedTiles.Add(board.MoveTile(tile, tile.Position.x, y2));

            foreach (Tile tile in row2)
                movedTiles.Add(board.MoveTile(tile, tile.Position.x, y1));

            CheckWhetherReactionIsAllowed();

            return new PermutationStep(movedTiles);
        }

        public PermutationStep SwapColumns(int x1, int x2)
        {
            List<Tile> column1 = board.GetColumn(x1).ToList();
            List<Tile> column2 = board.GetColumn(x2).ToList();

            var movedTiles = new List<MovedTile>();

            foreach (Tile tile in column1)
                movedTiles.Add(board.MoveTile(tile, x2, tile.Position.y));

            foreach (Tile tile in column2)
                movedTiles.Add(board.MoveTile(tile, x1, tile.Position.y));

            CheckWhetherReactionIsAllowed();

            return new PermutationStep(movedTiles);
        }

        public WildcardStep CreateWildcard(Vector2Int pos)
        {
            Tile tile = board[pos];

            if (tile.Inert)
                throw new InvalidOperationException("Cannot create wildcard from inert tile");

            if (tile.Wildcard)
                throw new InvalidOperationException("Tile was already a wildcard");
            
            tile.MakeWildcard();

            CheckWhetherReactionIsAllowed();

            return new WildcardStep(new List<Tile> { tile });
        }

        public PredictionStep TogglePrediction(Vector2Int pos)
        {
            Tile tile = board[pos];

            if (tile.Inert)
                throw new InvalidOperationException("Cannot mark inert tile for prediction");

            tile.Marked = !tile.Marked;

            if (tutorialTilesToOpen != null && tutorialTilesToOpen.All(t => t.Marked))
            {
                tutorialTilesToOpen = null;
                TutorialManager.Instance.CompleteActiveTutorial();
            }

            CheckWhetherReactionIsAllowed();

            return new PredictionStep(new List<Tile> { tile });
        }

        private void CheckWhetherReactionIsAllowed()
        {
            bool wasReactionAllowed = isReactionAllowed;

            isReactionAllowed = GetRawMatches().Count > 0;

            if (isReactionAllowed && !wasReactionAllowed)
                ReactionUnlocked?.Invoke();
            else if (wasReactionAllowed && !isReactionAllowed)
                ReactionLocked?.Invoke();
        }
    }
}