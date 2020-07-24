using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GMTK2020.Data
{
    public class Level
    {
        public Tile[,] Grid { get; }

        public List<Vector2Int> MatchingPattern { get; }

        public Level(Tile[,] grid, List<Vector2Int> pattern)
        {
            Grid = grid;
            MatchingPattern = pattern;
        }

        public override string ToString()
        {
            var message = new StringBuilder();
            for (int y = Grid.GetLength(1) - 1; y >= 0; --y)
            {
                for (int x = 0; x < Grid.GetLength(0); ++x)
                {
                    message.Append($"{Grid[x, y]?.Color ?? 0}\t");
                }
                message.AppendLine("");
            }
            return message.ToString();
        }
    }
}