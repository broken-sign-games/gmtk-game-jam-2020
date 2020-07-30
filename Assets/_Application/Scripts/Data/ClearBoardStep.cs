using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    public class ClearBoardStep
    {
        public List<Tile> ExtraneousPredictions { get; }
        public int ClearedRows { get; }
        public List<(Tile, Vector2Int)> ClearedTiles { get; }
        public List<(Tile, Vector2Int)> MovingTiles { get; }
        public List<(Tile, Vector2Int)> NewTiles { get; }

        public ClearBoardStep(List<Tile> extraneousPredictions, int clearedRows, List<(Tile, Vector2Int)> clearedTiles, List<(Tile, Vector2Int)> movingTiles, List<(Tile, Vector2Int)> newTiles)
        {
            ClearedRows = clearedRows;
            ExtraneousPredictions = extraneousPredictions;
            ClearedTiles = clearedTiles;
            MovingTiles = movingTiles;
            NewTiles = newTiles;
        }
    }
}