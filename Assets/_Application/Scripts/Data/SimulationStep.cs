using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    public class SimulationStep
    {
        public HashSet<Tile> MatchedTiles { get; }
        public List<(Tile, Vector2Int)> MovingTiles { get; }

        public SimulationStep(HashSet<Tile> matchedTiles, List<(Tile, Vector2Int)> movingTiles)
        {
            MatchedTiles = matchedTiles;
            MovingTiles = movingTiles;
        }
    } 
}