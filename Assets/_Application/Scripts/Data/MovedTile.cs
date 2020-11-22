using UnityEngine;

namespace GMTK2020.Data
{
    public struct MovedTile
    {
        public Tile Tile;
        public Vector2Int From;
        public Vector2Int To;

        public MovedTile(Tile tile, Vector2Int from)
            : this(tile, from, tile.Position) { }

        public MovedTile(Tile tile, Vector2Int from, Vector2Int to)
        {
            Tile = tile;
            From = from;
            To = to;
        }
    }
}