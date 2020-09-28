using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    public class Board : IEnumerable<Tile>
    {
        public int Width { get; }
        public int Height { get; }

        private readonly Tile[,] tiles;

        public Board(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;

            tiles = new Tile[Width, Height];
        }

        public Tile this[int x, int y]
        {
            get => tiles[x, y];

            set
            {
                value.Position = new Vector2Int(x, y);
                tiles[x, y] = value;
            }
        }

        public Tile this[Vector2Int pos]
        {
            get => tiles[pos.x, pos.y];

            set
            {
                value.Position = pos;
                tiles[pos.x, pos.y] = value;
            }
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            for (int x = 0; x < Width; ++x)
                for (int y = 0; y < Width; ++y)
                {
                    Tile tile = tiles[x, y];
                    if (tile != null)
                        yield return tile;
                }
        }

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    } 
}