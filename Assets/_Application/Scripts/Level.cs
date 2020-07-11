using System;
using System.Text;
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

    public class Tile
    {
        public string Color { get; }

        public Tile(string color)
        {
            Color = color;
        }
    }

}