using System;
using System.Collections.Generic;
using System.Text;
using UnityEditorInternal;
using UnityEngine;

namespace GMTK2020
{
    public class Level
    {
        public Tile[,] Grid { get; }

        public Level(Tile[,] grid)
        {
            Grid = grid;
        }

        public void PrintToConsole()
        {
            var message = new StringBuilder();
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.GetLength(1); y++)
                {
                    message.Append(Grid[x, y].Color + "\t");
                }
                message.AppendLine("");
            }
            Debug.Log(message.ToString());
        }
    }

    public class Main
    {

        private Tile[,] GenerateLevel()
        {
            throw new NotImplementedException();
        }

        private Prediction GetPredictionsFromHumansBrain()
        {
            throw new NotImplementedException();
        }

        private BetterLevel GenerateValidLevel(Simulator simulator)
        {
            // TODO: find legal simulation
            Tile[,] grid = GenerateLevel();
            Simulation simulation = simulator.Simulate(grid);
            return new BetterLevel(grid, simulation);
        }

        public void runIt()
        {
            Simulator simulator = new Simulator();
            Renderer renderer = new Renderer();


            BetterLevel level = GenerateValidLevel(simulator);

            renderer.RenderInitial(level.Grid);

            // It's the Human's turn.
            Validator validator = new Validator();
            Prediction prediction = GetPredictionsFromHumansBrain();
            int correctSteps = validator.ValidatePrediction(level.Simulation, prediction);

            renderer.RenderSimulation(level.Simulation, correctSteps);
        }
    }

    public class BetterLevel
    {

        public Tile[,] Grid { get; }
        public Simulation Simulation { get; }
        public BetterLevel(Tile[,] grid, Simulation simulation)
        {
            Grid = grid;
            Simulation = simulation;
        }
    }

    public class Renderer
    {
        public void RenderInitial(Tile[,] grid)
        {

        }

        public void RenderSimulation(Simulation simulation, int correctPredictions)
        {

        }
    }

    public class Simulator
    {
        public Simulation Simulate(Tile[,] grid)
        {
            return null;
        }
    }

    public class Validator
    {
        public int ValidatePrediction(Simulation simulation, Prediction prediction)
        {
            return 5;
        }
    }

    public class Prediction
    {
        public List<HashSet<Tile>> MatchedTilesPerStep { get; set; }
    }

    public class Simulation
    {
        public List<SimulationStep> Steps { get; }
    }

    public class SimulationStep
    {
        public HashSet<Tile> MatchedTiles { get; }
        public List<(Tile, Vector2Int)> MovingTiles { get; }
    }

    public class Tile
    {
        public string Color { get; }

        public Tile(string color)
        {
            Color = color;
        }
    }

}