using GMTK2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public class LevelLoader : MonoBehaviour
    {
        private void Start()
        {
            RunIt();
        }

        public void RunIt()
        {
            var level1Pattern = new HashSet<Vector2Int>()
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0)
            };
            Simulator simulator = new Simulator(level1Pattern);
            Renderer renderer = new Renderer();

            Level level = GenerateValidLevel(simulator);

            renderer.RenderInitial(level.Grid);

            // It's the Human's turn.
            Validator validator = new Validator();
            Prediction prediction = GetPredictionsFromHumansBrain();
            int correctSteps = validator.ValidatePrediction(level.Simulation, prediction);

            renderer.RenderSimulation(level.Simulation, correctSteps);
        }

        private Level GenerateValidLevel(Simulator simulator)
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
            var tiles = new Tile[9, 9];
            for (int x = 0; x < tiles.GetLength(0); ++x)
            {
                for (int y = 0; y < tiles.GetLength(1); ++y)
                {
                    tiles[x, y] = new Tile(rand.Next(0, 9));
                }
            }
            return tiles;
        }

        private Prediction GetPredictionsFromHumansBrain()
        {
            throw new NotImplementedException();
        }
    } 
}