using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA;
using Random = System.Random;

namespace GMTK2020
{
    public class Simulator
    {
        public const int MAX_SIMULATION_STEPS = 5;

        private readonly Board board;

        private readonly Random rng;
        private readonly int colorCount;

        public Simulator(Board initialBoard, int colorCount)
        {
            board = initialBoard;
            this.colorCount = colorCount;

            // TODO: We probably want more control over the seed...
            rng = new Random(Time.frameCount);
        }

        public SimulationStep SimulateNextStep()
        {
            HashSet<Tile> matchedTiles = RemoveMatchedTiles();

            if (matchedTiles.Count > 0)
            {
                List<MovedTile> movedTiles = MoveTilesDown();

                return new MatchStep(matchedTiles, movedTiles);
            }

            HashSet<Tile> inertTiles = MakeMarkedTilesInert();
            List<MovedTile> newTiles = FillBoardWithTiles();

            return new CleanUpStep(newTiles, inertTiles);
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

        public HashSet<Tile> RemoveMatchedTiles()
        {
            var matchedTiles = new HashSet<Tile>();

            foreach (IEnumerable<Tile> tiles in GetPotentialMatches())
            {
                if (!IsMatch(tiles))
                    continue;

                foreach (Tile tile in tiles)
                    matchedTiles.Add(tile);
            }

            foreach (Tile tile in matchedTiles)
                board[tile.Position] = null;

            return matchedTiles;
        }

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

        private HashSet<Tile> MakeMarkedTilesInert()
        {
            var inertTiles = new HashSet<Tile>();

            foreach (Tile tile in board)
            {
                if (tile.Marked)
                {
                    tile.MakeInert();
                    inertTiles.Add(tile);
                }
            }

            return inertTiles;
        }

        private List<MovedTile> FillBoardWithTiles()
        {
            var newTiles = new List<MovedTile>();

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

                    Tile newTile = new Tile(rng.Next(colorCount), new Vector2Int(x, y + newTilesInColumn));
                    newTiles.Add(board.MoveTile(newTile, x, y));
                }
            }

            return newTiles;
        }

        public RemovalStep RemoveTile(Vector2Int pos)
        {
            var positions = new List<Vector2Int>() { pos };

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

        public RemovalStep RemoveRow(int y)
        {
            List<Vector2Int> positions = board.GetXs().Select(x => new Vector2Int(x, y)).ToList();

            return RemoveTiles(positions);
        }

        public RemovalStep RemoveColor(int color)
        {
            List<Vector2Int> positions = board
                .Where(t => t.Color == color)
                .Select(t => t.Position)
                .ToList();

            return RemoveTiles(positions);
        }

        private RemovalStep RemoveTiles(List<Vector2Int> positions)
        {
            var removedTiles = new HashSet<Tile>();

            foreach (Vector2Int pos in positions)
            {
                removedTiles.Add(board[pos]);
                board[pos] = null;
            }

            List<MovedTile> movedTiles = MoveTilesDown();
            List<MovedTile> newTiles = FillBoardWithTiles();

            return new RemovalStep(removedTiles, movedTiles, newTiles);
        }

        public bool RefillTile(Vector2Int pos)
        {
            Tile tile = board[pos];

            bool wasInert = tile.Inert;
            if (wasInert)
                tile.Refill();

            return wasInert;
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

            return new PermutationStep(movedTiles);
        }

        public bool CreateWildcard(Vector2Int pos)
        {
            Tile tile = board[pos];

            bool wasWildcardOrInert = tile.Wildcard || tile.Inert;
            if (!wasWildcardOrInert)
                tile.MakeWildcard();

            return wasWildcardOrInert;
        }
    }
}