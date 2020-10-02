using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK2020
{
    public class Simulator
    {
        public const int MAX_SIMULATION_STEPS = 5;
        private readonly List<HashSet<Vector2Int>> matchingPatterns;

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

        public Simulation Simulate(Board initialBoard, bool allowShorterLevel = false)
        {
            var simulationSteps = new List<SimulationStep>();

            var workingBoard = new Board(initialBoard);

            for (int i = 0; i < MAX_SIMULATION_STEPS; ++i)
            {
                HashSet<(Tile, Vector2Int)> matchedTiles = RemoveMatchedTiles(workingBoard);
                if (matchedTiles.Count == 0)
                {
                    if (allowShorterLevel)
                        break;
                    else
                        throw new ArgumentException("Boring level.");
                }

                List<(Tile, Vector2Int)> movingTiles = MoveTilesDown(workingBoard);

                simulationSteps.Add(new SimulationStep(matchedTiles, movingTiles));
            }

            return new Simulation(simulationSteps);
        }

        public HashSet<(Tile, Vector2Int)> RemoveMatchedTiles(Board workingBoard)
        {
            var matchedTiles = new HashSet<(Tile, Vector2Int)>();

            int width = workingBoard.Width;
            int height = workingBoard.Height;

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = workingBoard[x, y];
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
                                || workingBoard[pos.x, pos.y]?.Color != tile.Color)
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

                    HashSet<Vector2Int> matchedPositions = ExpandMatch(workingBoard, origin);

                    foreach (Vector2Int pos in matchedPositions)
                    {
                        matchedTiles.Add((workingBoard[pos.x, pos.y], pos));
                        workingBoard[pos.x, pos.y] = null;
                    }
                }

            return matchedTiles;
        }

        private HashSet<Vector2Int> ExpandMatch(Board workingBoard, Vector2Int origin)
        {
            int width = workingBoard.Width;
            int height = workingBoard.Height;
            int color = workingBoard[origin.x, origin.y].Color;

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
                        && workingBoard[candidate.x, candidate.y]?.Color == color)
                    {
                        positionsToProcess.Enqueue(candidate);
                        fullMatch.Add(candidate);
                    }
                }
            }

            return fullMatch;
        }

        public List<(Tile, Vector2Int)> MoveTilesDown(Board workingBoard)
        {
            int width = workingBoard.Width;
            int height = workingBoard.Height;

            var movedTiles = new List<(Tile, Vector2Int)>();

            for (int x = 0; x < width; ++x)
            {
                int top = 0;
                for (int y = 0; y < height; ++y)
                {
                    Tile tile = workingBoard[x, y];
                    if (tile is null)
                        continue;

                    if (y > top)
                    {
                        movedTiles.Add((tile, new Vector2Int(x, top)));
                        workingBoard[x, top] = tile;
                        workingBoard[x, y] = null;
                    }

                    ++top;
                }
            }

            return movedTiles;
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