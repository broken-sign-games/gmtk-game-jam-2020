using System;

namespace GMTK2020.Data
{
    public class Board
    {
        public int Width { get; }
        public int Height { get; }

        public Board(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
        }
    } 
}