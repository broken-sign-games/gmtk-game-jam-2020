using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;
        [SerializeField] private PatternRenderer patternRenderer = null;
        [SerializeField] private PredictionEditor predictionEditor = null;

        public Level Level { get; private set; }

        private void Start()
        {
            RunIt();
        }

        public void RunIt()
        {
            var level1Pattern = new HashSet<Vector2Int>()
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, 1),
            };
            Simulator simulator = new Simulator(level1Pattern);

            Level = GenerateValidLevel(simulator);

            predictionEditor.Initialize(Level.Grid);
            boardRenderer.RenderInitial(Level.Grid);
            patternRenderer.RenderPattern(level1Pattern);
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
                    tiles[x, y] = new Tile(rand.Next(0, 5));
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