using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    public class SimulationStep
    {
        public HashSet<Tile> MatchedTiles { get; }
        public List<Tile> MovingTiles { get; }

        public SimulationStep(HashSet<Tile> matchedTiles, List<Tile> movingTiles)
        {
            MatchedTiles = matchedTiles;
            MovingTiles = movingTiles;
        }
    } 
}