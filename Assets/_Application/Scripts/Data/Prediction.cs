using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Prediction
    {
        public List<HashSet<Tile>> MatchedTilesPerStep { get; }

        public Prediction(List<HashSet<Tile>> matchedTilesPerStep)
        {
            MatchedTilesPerStep = matchedTilesPerStep;
        }
    }
}
