using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using UnityEngine;
using Random = System.Random;

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
                    message.Append($"{Grid[x, y].Color}\t");
                }
                message.AppendLine("");
            }
            Debug.Log(message.ToString());
        }
    }

    public class Main
    {

        private Prediction GetPredictionsFromHumansBrain()
        {
            throw new NotImplementedException();
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

        private BetterLevel GenerateValidLevel(Simulator simulator)
        {
            // TODO: find legal simulation
            Tile[,] grid = GenerateLevel();
            Simulation simulation = simulator.Simulate(grid);
            return new BetterLevel(grid, simulation);
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
        private const int MAX_SIMULATION_STEPS = 5;
        private List<HashSet<Vector2Int>> matchingPatterns;

        public Simulator(HashSet<Vector2Int> matchingPattern)
        {
            matchingPatterns = new List<HashSet<Vector2Int>>() { matchingPattern };

            for (int i = 0; i < 3; ++i)
            {
                matchingPattern = RotatePattern(matchingPattern);
                matchingPatterns.Add(matchingPattern);
            }
            
            matchingPattern = MirrorPattern(matchingPattern);
            matchingPatterns.Add(matchingPattern);

            for (int i = 0; i < 3; ++i)
            {
                matchingPattern = RotatePattern(matchingPattern);
                matchingPatterns.Add(matchingPattern);
            }
        }

        public Simulation Simulate(Tile[,] initialGrid)
        {
            var simulationSteps = new List<SimulationStep>();

            Tile[,] workingGrid = initialGrid.Clone() as Tile[,];

            for (int i = 0; i < MAX_SIMULATION_STEPS; ++i)
            {
                HashSet<Tile> matchedTiles = RemoveMatchedTiles(workingGrid);
                if (matchedTiles.Count == 0)
                {
                    throw new ArgumentException("Boring level.");
                }

                List<(Tile, Vector2Int)> movingTiles = MoveTilesDown(workingGrid);

                simulationSteps.Add(new SimulationStep(matchedTiles, movingTiles));
            }

            return new Simulation(simulationSteps);
        }

        private HashSet<Tile> RemoveMatchedTiles(Tile[,] workingGrid)
        {
            HashSet<Tile> matchedTiles = new HashSet<Tile>();

            int width = workingGrid.GetLength(0);
            int height = workingGrid.GetLength(1);

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = workingGrid[x, y];
                    if (tile is null)
                        continue;

                    Vector2Int origin = new Vector2Int(x, y);

                    bool isMatch = false;
                    foreach (HashSet<Vector2Int> pattern in matchingPatterns)
                    {
                        bool doesThisSymmetryMatch = true;
                        foreach (Vector2Int offset in pattern)
                        {
                            if (offset == Vector2Int.zero)
                                continue;

                            Vector2Int pos = origin + offset;
                            if (pos.x < 0
                                || pos.y < 0
                                || pos.x >= width
                                || pos.y >= width
                                || workingGrid[pos.x, pos.y]?.Color != tile.Color)
                            {
                                doesThisSymmetryMatch = false;
                                break;
                            }
                        }

                        if (doesThisSymmetryMatch)
                        {
                            isMatch = true;
                            break;
                        }
                    }

                    if (!isMatch)
                        continue;

                    HashSet<Vector2Int> matchedPositions = ExpandMatch(workingGrid, origin);

                    foreach (Vector2Int pos in matchedPositions)
                    {
                        matchedTiles.Add(workingGrid[pos.x, pos.y]);
                        workingGrid[pos.x, pos.y] = null;
                    }
                }

            return matchedTiles;
        }

        private HashSet<Vector2Int> ExpandMatch(Tile[,] workingGrid, Vector2Int origin)
        {
            int width = workingGrid.GetLength(0);
            int height = workingGrid.GetLength(1);
            int color = workingGrid[origin.x, origin.y].Color;

            var fullMatch = new HashSet<Vector2Int>() { origin };

            var positionsToProcess = new Queue<Vector2Int>();
            positionsToProcess.Enqueue(origin);

            var neighborhood = new HashSet<Vector2Int>()
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1)
            };

            while (positionsToProcess.Count > 0)
            {
                Vector2Int pos = positionsToProcess.Dequeue();

                foreach (Vector2Int offset in neighborhood)
                {
                    Vector2Int candidate = pos + offset;
                    if (fullMatch.Contains(candidate))
                        continue;

                    if (candidate.x >= 0
                        && candidate.y >= 0
                        && candidate.x < width
                        && candidate.y < height
                        && workingGrid[candidate.x, candidate.y]?.Color == color)
                    {
                        positionsToProcess.Enqueue(candidate);
                        fullMatch.Add(pos);
                    }
                }
            }

            return fullMatch;
        }

        private List<(Tile, Vector2Int)> MoveTilesDown(Tile[,] workingGrid)
        {
            throw new NotImplementedException();
        }

        private HashSet<Vector2Int> MirrorPattern(HashSet<Vector2Int> pattern)
        { 
            return new HashSet<Vector2Int>(pattern.Select((vec2) =>
            {
                (int x, int y) = vec2;
                return new Vector2Int(x, -y);
            }));
        }

        private HashSet<Vector2Int> RotatePattern(HashSet<Vector2Int> pattern)
        {
            return new HashSet<Vector2Int>(pattern.Select((vec2) =>
            {
                (int x, int y) = vec2;
                return new Vector2Int(y, -x);
            }));
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

        public Simulation(List<SimulationStep> steps)
        {
            Steps = steps;
        }
    }

    public class SimulationStep
    {
        public HashSet<Tile> MatchedTiles { get; }
        public List<(Tile, Vector2Int)> MovingTiles { get; }

        public SimulationStep(HashSet<Tile> matchedTiles, List<(Tile, Vector2Int)> movingTiles)
        {
            MatchedTiles = matchedTiles;
            MovingTiles = movingTiles;
        }
    }

    public sealed class Tile
    {
        public int Color { get; }

        public Tile(int color)
        {
            Color = color;
        }
    }

}