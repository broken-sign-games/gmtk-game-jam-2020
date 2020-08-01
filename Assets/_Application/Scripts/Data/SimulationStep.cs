using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    public class SimulationStep
    {
        public HashSet<(Tile, Vector2Int)> MatchedTiles { get; }
        public List<(Tile, Vector2Int)> MovingTiles { get; }
        public List<(Tile, Vector2Int)> NewTiles { get; }
        public int Score { get; set; }

        public SimulationStep(HashSet<(Tile, Vector2Int)> matchedTiles, List<(Tile, Vector2Int)> movingTiles, List<(Tile, Vector2Int)> newTiles)
        {
            MatchedTiles = matchedTiles;
            MovingTiles = movingTiles;
            NewTiles = newTiles;
        }
    }
}