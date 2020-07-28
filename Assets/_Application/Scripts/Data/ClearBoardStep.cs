using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    public class ClearBoardStep
    {
        public int ClearedRows { get; }
        public List<(Tile, Vector2Int)> ClearedTiles { get; }
        public List<(Tile, Vector2Int)> MovingTiles { get; }
        public List<(Tile, Vector2Int)> NewTiles { get; }

        public ClearBoardStep(int clearedRows, List<(Tile, Vector2Int)> clearedTiles, List<(Tile, Vector2Int)> movingTiles, List<(Tile, Vector2Int)> newTiles)
        {
            ClearedRows = clearedRows;
            ClearedTiles = clearedTiles;
            MovingTiles = movingTiles;
            NewTiles = newTiles;
        }
    }
}