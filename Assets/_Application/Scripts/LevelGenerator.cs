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
            Tile[,] grid;
            Simulation simulation = null;
            bool isValid;
            do
            {
                grid = GenerateLevel();
                isValid = true;
                try
                {
                    simulation = simulator.Simulate(grid);
                    isValid = ValidateSimulation(simulation);
                }
                catch (ArgumentException)
                {
                    isValid = false;
                }
            }
            while (!isValid);

            return new Level(grid, simulation);
        }

        private Tile[,] GenerateLevel()
        {
            switch (levelSpec.GeneratorStrategy)
            {
            case GeneratorStrategy.Random: return GenerateRandomLevel();
            case GeneratorStrategy.SingleHorizontalMatch: return GenerateSingleHorizontalMatchLevel();
            default: throw new InvalidOperationException("Unknown level generation strategy.");
            }
        }

        private Tile[,] GenerateRandomLevel()
        {
            var rand = new Random();
            var tiles = new Tile[levelSpec.Size.x, levelSpec.Size.y];
            for (int x = 0; x < levelSpec.Size.x; ++x)
            {
                for (int y = 0; y < levelSpec.Size.y; ++y)
                {
                    tiles[x, y] = new Tile(rand.Next(levelSpec.ColorCount));
                }
            }
            return tiles;
        }

        private Tile[,] GenerateSingleHorizontalMatchLevel()
        {
            var rng = new Random();
            int width = levelSpec.Size.x;
            int height = levelSpec.Size.y;
            var tiles = new Tile[width, height];
            
            List<int> colors = Enumerable.Range(0, levelSpec.ColorCount).ToList().Shuffle(rng);

            foreach (int color in colors)
            {
                int leftEnd;
                do
                {
                    leftEnd = rng.Next(width - 1);
                }
                while (tiles[leftEnd, height - 1] != null 
                    || tiles[leftEnd + 1, height - 1] != null);

                for (int x = leftEnd; x < leftEnd + 2; ++x)
                    for (int y = 0; y < height; ++y)
                    {
                        if (tiles[x, y] is null)
                        {
                            tiles[x, y] = new Tile(color);
                            break;
                        }
                    }
            }

            return tiles;
        }

        private void FillGridWithNonMatchingTiles(Tile[,] tiles, Random rng)
        {
            for (int y = 0; y < tiles.GetLength(1); ++y)
                for (int x = 0; x < tiles.GetLength(0); ++x)
                    if (tiles[x, y] is null)
                        tiles[x, y] = GetNonMatchingTile(x, y, tiles, rng);
        }

        private Tile GetNonMatchingTile(int x, int y, Tile[,] tiles, Random rng)
        {
            var neighborhood = new HashSet<Vector2Int>
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
            };

            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);

            var colors = new HashSet<int>(Enumerable.Range(0, levelSpec.ColorCount));
            var origin = new Vector2Int(x, y);
            foreach (Vector2Int offset in neighborhood)
            {
                Vector2Int pos = origin + offset;
                if (pos.x < 0
                    || pos.y < 0
                    || pos.x >= width
                    || pos.y >= height
                    || tiles[pos.x, pos.y] is null)
                {
                    continue;
                }
                colors.Remove(tiles[pos.x, pos.y].Color);
            }

            int color = colors.ElementAt(rng.Next(colors.Count));
            return new Tile(color);
        }

        private bool ValidateSimulation(Simulation simulation)
        {
            switch (levelSpec.GeneratorStrategy)
            {
            case GeneratorStrategy.Random: return true;
            case GeneratorStrategy.SingleHorizontalMatch: return ValidateSingleHorizontalMatches(simulation);
            default: return true;
            }
        }

        private bool ValidateSingleHorizontalMatches(Simulation simulation)
        {
            foreach (SimulationStep step in simulation.Steps)
            {
                (Tile tile, Vector2Int pos)[] tiles = step.MatchedTiles.ToArray();

                if (tiles.Length != 2)
                    return false;

                Vector2Int delta = tiles[0].pos - tiles[1].pos;

                if (delta.y != 0 || Math.Abs(delta.x) != 1)
                    return false;
            }

            return true;
        }
    }
}