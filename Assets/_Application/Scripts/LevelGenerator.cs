using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public class LevelGenerator
    {
        private LevelSpecification levelSpec;
        private Simulator simulator;

        public LevelGenerator(LevelSpecification levelSpec, Simulator simulator)
        {
            this.levelSpec = levelSpec;
            this.simulator = simulator;
        }

        public Level GenerateValidLevel()
        {
            Board board = null;
            Simulation simulation = null;
            bool isValid;
            do
            {
                isValid = true;
                try
                {
                    board = GenerateLevel();
                    simulation = simulator.Simulate(board.DeepCopy());
                    isValid = ValidateSimulation(simulation);
                }
                catch (Exception e) when (
                    e is ArgumentException ||
                    e is InvalidOperationException)
                {
                    isValid = false;
                }
            }
            while (!isValid);

            return new Level(board, simulation);
        }

        private Board GenerateLevel()
        {
            switch (levelSpec.GeneratorStrategy)
            {
            case GeneratorStrategy.Random: return GenerateRandomLevel();
            case GeneratorStrategy.SingleHorizontalMatch: return GenerateSingleHorizontalMatchLevel();
            case GeneratorStrategy.SingleMatch: return GenerateSingleMatchLevel();
            case GeneratorStrategy.MultipleMatches: return GenerateSingleMatchLevel();
            case GeneratorStrategy.BiggerMatches: return GenerateSingleMatchLevel();
            default: throw new InvalidOperationException("Unknown level generation strategy.");
            }
        }

        private Board GenerateRandomLevel()
        {
            var rand = new Random();
            var board = new Board(levelSpec.Size.x, levelSpec.Size.y);
            for (int x = 0; x < levelSpec.Size.x; ++x)
            {
                for (int y = 0; y < levelSpec.Size.y; ++y)
                {
                    board[x, y] = new Tile(rand.Next(levelSpec.ColorCount));
                }
            }
            return board;
        }

        private Board GenerateSingleHorizontalMatchLevel()
        {
            var rng = new Random();
            int width = levelSpec.Size.x;
            int height = levelSpec.Size.y;
            var board = new Board(width, height);

            List<int> colors = Enumerable.Range(0, levelSpec.ColorCount).ToList().Shuffle(rng);

            var anchors = new List<Vector2Int> { new Vector2Int(width / 2, 0) };

            for (int i = 0; i < colors.Count; ++i)
            {
                int color = colors[i];
                int leftEnd;
                if (anchors.Count == 1)
                {
                    leftEnd = anchors[0].x == 0 ? 0 :
                                anchors[0].x == width - 1 ? width - 2 :
                                anchors[0].x - rng.Next(2);
                }
                else
                {
                    leftEnd = anchors[0].x == 0 ? 1 :
                                anchors[1].x == width - 1 ? width - 3 :
                                anchors[1].x - 2 * rng.Next(2);
                }

                if (board[leftEnd, height - 1] != null || board[leftEnd + 1, height - 1] != null)
                    throw new InvalidOperationException("Can't fit horizontal match");

                var newTiles = new Vector2Int[2]
                {
                    new Vector2Int(leftEnd, anchors[0].y),
                    new Vector2Int(leftEnd + 1, anchors[0].y)
                };

                anchors = newTiles.ToList();

                foreach (Vector2Int tile in newTiles)
                {
                    for (int y = height - 1; y > tile.y; --y)
                    {
                        board[tile.x, y] = board[tile.x, y - 1];
                    }
                    board[tile.x, tile.y] = new Tile(color);
                }
            }

            FillBoardWithNonMatchingTiles(board, rng);

            return board;
        }

        private Board GenerateSingleMatchLevel()
        {
            var rng = new Random();
            int width = levelSpec.Size.x;
            int height = levelSpec.Size.y;
            var board = new Board(width, height);

            List<int> colors = Enumerable.Range(0, levelSpec.ColorCount).ToList().Shuffle(rng);
            List<bool> verticalList = new List<bool> { true, true, false, false, false }.Shuffle(rng);

            var anchors = new List<Vector2Int> { new Vector2Int(width / 2, 0) };

            for (int i = 0; i < colors.Count; ++i)
            {
                int color = colors[i];
                bool vertical = verticalList[i];

                var newTiles = new Vector2Int[2];
                if (vertical)
                {
                    Vector2Int anchor = anchors[rng.Next(anchors.Count)];
                    if (board[anchor.x, height - 1] != null || board[anchor.x, height - 2] != null)
                        throw new InvalidOperationException("Can't fit vertical match");

                    newTiles[0] = anchor;
                    newTiles[1] = anchor + new Vector2Int(0, 1);

                    anchors = new List<Vector2Int> { newTiles[1] };
                }
                else
                {
                    int leftEnd;
                    if (anchors.Count == 1)
                    {
                        leftEnd = anchors[0].x == 0 ? 0 :
                                  anchors[0].x == width - 1 ? width - 2 :
                                  anchors[0].x - rng.Next(2);
                    }
                    else
                    {
                        leftEnd = anchors[0].x == 0 ? 1 :
                                  anchors[1].x == width - 1 ? width - 3 :
                                  anchors[1].x - 2 * rng.Next(2);
                    }

                    if (board[leftEnd, height - 1] != null || board[leftEnd + 1, height - 1] != null)
                        throw new InvalidOperationException("Can't fit horizontal match");

                    newTiles[0] = new Vector2Int(leftEnd, anchors[0].y);
                    newTiles[1] = new Vector2Int(leftEnd+1, anchors[0].y);

                    anchors = newTiles.ToList();
                }

                foreach (Vector2Int tile in newTiles)
                {
                    for (int y = height - 1; y > tile.y; --y)
                    {
                        board[tile.x, y] = board[tile.x, y - 1];
                    }
                    board[tile] = new Tile(color, tile);
                }
            }

            FillBoardWithNonMatchingTiles(board, rng);

            return board;
        }

        private void FillBoardWithNonMatchingTiles(Board board, Random rng)
        {
            foreach (int y in board.GetYs())
                foreach (int x in board.GetXs())
                    if (board[x, y] is null)
                        board[x, y] = GetNonMatchingTile(x, y, board, rng);
        }

        private Tile GetNonMatchingTile(int x, int y, Board board, Random rng)
        {
            var neighborhood = new HashSet<Vector2Int>
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
            };

            int width = board.Width;
            int height = board.Height;

            var colors = new HashSet<int>(Enumerable.Range(0, levelSpec.ColorCount));
            var origin = new Vector2Int(x, y);
            foreach (Vector2Int offset in neighborhood)
            {
                Vector2Int pos = origin + offset;
                if (pos.x < 0
                    || pos.y < 0
                    || pos.x >= width
                    || pos.y >= height
                    || board[pos] is null)
                {
                    continue;
                }
                colors.Remove(board[pos].Color);
            }

            int color = colors.ElementAt(rng.Next(colors.Count));
            return new Tile(color, origin);
        }

        private bool ValidateSimulation(Simulation simulation)
        {
            switch (levelSpec.GeneratorStrategy)
            {
            case GeneratorStrategy.Random: return true;
            case GeneratorStrategy.SingleHorizontalMatch: return ValidateSingleHorizontalMatches(simulation);
            case GeneratorStrategy.SingleMatch: return ValidateSingleMatches(simulation);
            case GeneratorStrategy.MultipleMatches: return ValidateMultipleMatches(simulation);
            case GeneratorStrategy.BiggerMatches: return ValidateBiggerMatches(simulation);
            default: return true;
            }
        }

        private bool ValidateSingleHorizontalMatches(Simulation simulation)
        {
            foreach (SimulationStep step in simulation.Steps)
            {
                Tile[] tiles = step.MatchedTiles.ToArray();

                if (tiles.Length != 2)
                    return false;

                Vector2Int delta = tiles[0].Position - tiles[1].Position;

                if (delta.y != 0 || Math.Abs(delta.x) != 1)
                    return false;
            }

            return true;
        }

        private bool ValidateSingleMatches(Simulation simulation)
        {
            foreach (SimulationStep step in simulation.Steps)
            {
                if (step.MatchedTiles.Count != 2)
                    return false;
            }

            return true;
        }

        private bool ValidateMultipleMatches(Simulation simulation)
        {
            bool foundMultiMatch = false;

            foreach (SimulationStep step in simulation.Steps)
            {
                int matches = 0;
                for (int color = 0; color < levelSpec.ColorCount; ++color)
                {
                    int tileCount = step.MatchedTiles.Count(t => t.Color == color);
                    if (tileCount == 0)
                        continue;

                    if (tileCount != 2)
                        return false;

                    ++matches;
                }

                if (matches == 0)
                    return false;

                if (matches > 1)
                    foundMultiMatch = true;
            }

            return foundMultiMatch;
        }

        private bool ValidateBiggerMatches(Simulation simulation)
        {
            int matchPatternSize = levelSpec.MatchingPattern.Count;
            foreach (SimulationStep step in simulation.Steps)
            {
                for (int color = 0; color < levelSpec.ColorCount; ++color)
                {
                    int tileCount = step.MatchedTiles.Count(t => t.Color == color);
                    if (tileCount % matchPatternSize != 0)
                        return true;
                }
            }

            return false;
        }
    }
}