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
            };
            var level1Size = new Vector2Int(6, 4);
            int level1ColorCount = 5;
            Simulator simulator = new Simulator(level1Pattern);

            Level = GenerateValidLevel(simulator, level1Size, level1ColorCount);

            predictionEditor.Initialize(Level.Grid);
            boardRenderer.RenderInitial(Level.Grid);
            patternRenderer.RenderPattern(level1Pattern);
        }

        private Level GenerateValidLevel(Simulator simulator, Vector2Int boardSize, int colorCount)
        {
            Tile[,] grid;
            Simulation simulation = null;
            bool isValid;
            do
            {
                grid = GenerateLevel(boardSize, colorCount);
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

        private Tile[,] GenerateLevel(Vector2Int boardSize, int colorCount)
        {
            var rand = new Random();
            var tiles = new Tile[boardSize.x, boardSize.y];
            for (int x = 0; x < boardSize.x; ++x)
            {
                for (int y = 0; y < boardSize.y; ++y)
                {
                    tiles[x, y] = new Tile(rand.Next(0, colorCount));
                }
            }
            return tiles;
        }
    } 
}