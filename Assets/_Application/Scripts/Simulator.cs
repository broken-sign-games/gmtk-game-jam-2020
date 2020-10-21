using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public class Simulator
    {
        public const int MAX_SIMULATION_STEPS = 5;

        private Board board;

        private Random rng;
        private int colorCount;

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

            foreach (int x in board.GetXs())
            {
                int matchStart = 0;
                foreach (int y in board.GetYs())
                {
                    Tile tile = board[x, y];
                    if (tile is null || tile.Inert || !tile.Marked)
                    {
                        matchStart = y + 1;
                        continue;
                    }

                    if (tile.Color != board[x, matchStart].Color)
                    {
                        matchStart = y;
                        continue;
                    }

                    if (y - matchStart == 2)
                    {
                        matchedTiles.Add(board[x, matchStart]);
                        matchedTiles.Add(board[x, matchStart + 1]);
                    }

                    if (y - matchStart >= 2)
                    {
                        matchedTiles.Add(board[x, y]);
                    }
                }
            }

            foreach (int y in board.GetYs())
            {
                int matchStart = 0;
                foreach (int x in board.GetXs())
                {
                    Tile tile = board[x, y];
                    if (tile is null || tile.Inert || !tile.Marked)
                    {
                        matchStart = x + 1;
                        continue;
                    }

                    if (tile.Color != board[matchStart, y].Color)
                    {
                        matchStart = x;
                        continue;
                    }

                    if (x - matchStart == 2)
                    {
                        matchedTiles.Add(board[matchStart, y]);
                        matchedTiles.Add(board[matchStart + 1, y]);
                    }

                    if (x - matchStart >= 2)
                    {
                        matchedTiles.Add(board[x, y]);
                    }
                }
            }

            foreach (Tile tile in matchedTiles)
                board[tile.Position] = null;

            return matchedTiles;
        }

        public bool FurtherMatchesPossible()
        {
            foreach (int x in board.GetXs())
            {
                int matchStart = 0;
                foreach (int y in board.GetYs())
                {
                    Tile tile = board[x, y];
                    if (tile is null || tile.Inert)
                    {
                        matchStart = y + 1;
                        continue;
                    }

                    if (tile.Color != board[x, matchStart].Color)
                    {
                        matchStart = y;
                        continue;
                    }

                    if (y - matchStart == 2)
                    {
                        return true;
                    }
                }
            }

            foreach (int y in board.GetYs())
            {
                int matchStart = 0;
                foreach (int x in board.GetXs())
                {
                    Tile tile = board[x, y];
                    if (tile is null || tile.Inert)
                    {
                        matchStart = x + 1;
                        continue;
                    }

                    if (tile.Color != board[matchStart, y].Color)
                    {
                        matchStart = x;
                        continue;
                    }

                    if (x - matchStart == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
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
    }
}