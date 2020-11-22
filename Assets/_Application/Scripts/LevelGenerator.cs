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

        private Random rng;
        
        public LevelGenerator(LevelSpecification levelSpec)
        {
            this.levelSpec = levelSpec;

            rng = new Random(Time.frameCount);
        }

        public Level GenerateValidLevel()
        {
            Board board = null;
            bool isValid;
            var failureReasons = new Dictionary<string, int>();
            string FAILED_VALIDATION = "Failed validation";
            int MAX_ATTEMPTS = 1000;
            int nAttempts = 0;
            do
            {
                ++nAttempts;
                isValid = true;
                try
                {
                    board = GenerateLevel();
                    Board workingCopy = board.DeepCopy();
                    foreach (Tile tile in workingCopy)
                        tile.Marked = true;
                    List<SimulationStep> steps = new Simulator(workingCopy, levelSpec.ColorCount).SimulateToStop();
                    isValid = ValidateSimulation(steps);

                    if (!isValid)
                        failureReasons[FAILED_VALIDATION] = failureReasons.GetValueOrDefault(FAILED_VALIDATION, 0) + 1;
                }
                catch (Exception e) when (
                    e is ArgumentException ||
                    e is InvalidOperationException)
                {
                    isValid = false;
                    failureReasons[e.Message] = failureReasons.GetValueOrDefault(e.Message, 0) + 1;
                }

                if (nAttempts >= MAX_ATTEMPTS)
                    isValid = true;
            }
            while (!isValid);

            foreach ((string reason, int count) in failureReasons)
            {
                Debug.Log($"{reason}: {count}");
            }

            return new Level(board);
        }

        private Board GenerateLevel()
        {
            return GenerateSingleMatchLevel();
        }

        private Board GenerateSingleMatchLevel()
        {
            int width = levelSpec.Size.x;
            int height = levelSpec.Size.y;
            var board = new Board(width, height);

            List<int> colors = Enumerable.Range(0, levelSpec.GuaranteedChain).ToList().Shuffle(rng);
            int verticalMatches = levelSpec.GuaranteedChain / 2;
            int horizontalMatches = levelSpec.GuaranteedChain - verticalMatches;

            List<bool> verticalList = Enumerable.Repeat(true, verticalMatches).ToList();
            verticalList.AddRange(
                Enumerable.Repeat(false, horizontalMatches)
            );
            verticalList = verticalList.Shuffle(rng);

            var anchors = new List<Vector2Int> { new Vector2Int(width / 2, 0) };

            for (int i = 0; i < colors.Count; ++i)
            {
                int color = colors[i];
                bool vertical = verticalList[i];

                var newTiles = new Vector2Int[3];
                if (vertical)
                {
                    Vector2Int anchor = anchors[rng.Next(anchors.Count)];
                    if (board[anchor.x, height - 1] != null || board[anchor.x, height - 2] != null || board[anchor.x, height - 3] != null)
                        throw new InvalidOperationException("Can't fit vertical match");

                    newTiles[0] = anchor;
                    newTiles[1] = anchor + new Vector2Int(0, 1);
                    newTiles[2] = anchor + new Vector2Int(0, 2);

                    anchors = new List<Vector2Int> { newTiles[1], newTiles[2] };
                }
                else
                {
                    Vector2Int anchor = anchors[rng.Next(anchors.Count)];
                    int leftEnd = Math.Max(0, Math.Min(width - 3, anchor.x - rng.Next(3)));

                    if (board[leftEnd, height - 1] != null || board[leftEnd + 1, height - 1] != null || board[leftEnd + 2, height - 1] != null)
                        throw new InvalidOperationException("Can't fit horizontal match");

                    newTiles[0] = new Vector2Int(leftEnd, anchor.y);
                    newTiles[1] = new Vector2Int(leftEnd+1, anchor.y);
                    newTiles[2] = new Vector2Int(leftEnd+2, anchor.y);

                    anchors = new List<Vector2Int>();

                    for (int y = anchor.y; y >= 0; --y)
                    {
                        anchors.Add(new Vector2Int(newTiles[0].x, y));
                        anchors.Add(new Vector2Int(newTiles[2].x, y));
                    }
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

            FillBoardWithNonMatchingTiles(board);

            return board;
        }

        private void FillBoardWithNonMatchingTiles(Board board)
        {
            foreach (int y in board.GetYs())
                foreach (int x in board.GetXs())
                    if (board[x, y] is null)
                        board[x, y] = GetNonMatchingTile(x, y, board);
        }

        private Tile GetNonMatchingTile(int x, int y, Board board)
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
                //colors.Remove(board[pos].Color);
            }

            int color = colors.ElementAt(rng.Next(colors.Count));
            return new Tile(color, origin);
        }

        private bool ValidateSimulation(List<SimulationStep> steps)
        {
            return steps.Count == levelSpec.GuaranteedChain + 1;
        }
    }
}