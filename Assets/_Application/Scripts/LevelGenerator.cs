using GMTK2020.Data;
using System;
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
            var rand = new Random();
            var tiles = new Tile[levelSpec.Size.x, levelSpec.Size.y];
            for (int x = 0; x < levelSpec.Size.x; ++x)
            {
                for (int y = 0; y < levelSpec.Size.y; ++y)
                {
                    tiles[x, y] = new Tile(rand.Next(0, levelSpec.ColorCount));
                }
            }
            return tiles;
        }
    }
}