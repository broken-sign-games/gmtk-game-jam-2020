using UnityEngine;

namespace GMTK2020.Data
{
    public sealed class Tile
    {
        public int Color { get; }

        public bool Inert { get; private set; }
        public bool Marked { get; set; }
        public Vector2Int Position { get; set; }

        public Tile(int color, Vector2Int position)
        {
            Color = color;
            Position = position;
        }

        public void MakeInert()
        {
            Inert = true;
            Marked = false;
        }
    }
}
