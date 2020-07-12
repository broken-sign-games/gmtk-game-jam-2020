using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    public class SimulationStep
    {
        public HashSet<(Tile, Vector2Int)> MatchedTiles { get; }
        public List<(Tile, Vector2Int)> MovingTiles { get; }

        public SimulationStep(HashSet<(Tile, Vector2Int)> matchedTiles, List<(Tile, Vector2Int)> movingTiles)
        {
            MatchedTiles = matchedTiles;
            MovingTiles = movingTiles;
        }
    } 
}