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
        private readonly List<HashSet<Vector2Int>> matchingPatterns;
        private Random rng;

        public Simulator(HashSet<Vector2Int> matchingPattern, Random rng)
        {
            this.rng = rng;
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

        public Simulation Simulate(Tile[,] initialGrid, Prediction prediction = null)
        {
            var simulationSteps = new List<SimulationStep>();

            Tile[,] workingGrid = initialGrid.Clone() as Tile[,];
            var remainingPredictions = new HashSet<Tile>(prediction.PredictedTiles);

            while (true)
            {
                HashSet<(Tile, Vector2Int)> matchedTiles = RemoveMatchedTiles(workingGrid, remainingPredictions);
                if (matchedTiles.Count == 0)
                {
                    break;
                }

                List<(Tile, Vector2Int)> movingTiles = MoveTilesDown(workingGrid);

                List<(Tile, Vector2Int)> newTiles = FillWithNewTiles(workingGrid);

                simulationSteps.Add(new SimulationStep(matchedTiles, movingTiles, newTiles));
            }

            int clearedRows = Math.Min(simulationSteps.Count - 1, 0);

            foreach (Tile tile in remainingPredictions)
                tile.Petrify();

            return new Simulation(simulationSteps, remainingPredictions.ToList(), clearedRows);
        }

        // TODO: Stop expanding matches, but allow overlapping matches.
        // TODO: Only match predicted tiles is predictions aren't null.
        public HashSet<(Tile, Vector2Int)> RemoveMatchedTiles(Tile[,] workingGrid, HashSet<Tile> remainingPredictions)
        {
            var matchedTiles = new HashSet<(Tile, Vector2Int)>();

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
                                || pos.y >= height
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
                        matchedTiles.Add((workingGrid[pos.x, pos.y], pos));
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
                        fullMatch.Add(candidate);
                    }
                }
            }

            return fullMatch;
        }

        public List<(Tile, Vector2Int)> MoveTilesDown(Tile[,] workingGrid)
        {
            int width = workingGrid.GetLength(0);
            int height = workingGrid.GetLength(1);

            var movedTiles = new List<(Tile, Vector2Int)>();

            for (int x = 0; x < width; ++x)
            {
                int top = 0;
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = workingGrid[x, y];
                    if (tile is null)
                        continue;

                    if (y > top)
                    {
                        movedTiles.Add((tile, new Vector2Int(x, top)));
                        workingGrid[x, top] = tile;
                        workingGrid[x, y] = null;
                    }

                    ++top;
                }
            }

            return movedTiles;
        }

        private List<(Tile, Vector2Int)> FillWithNewTiles(Tile[,] workingGrid)
        {
            int width = workingGrid.GetLength(0);
            int height = workingGrid.GetLength(1);

            var newTiles = new List<(Tile, Vector2Int)>();

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = workingGrid[x, y];
                    if (tile != null)
                        continue;

                    // TODO: Should this be the responsibility of the level generator?
                    tile = workingGrid[x, y] = new Tile(rng.Next(5));

                    newTiles.Add((tile, new Vector2Int(x, y)));
                }
            }

            return newTiles;
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
}