using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Prediction
    {
        public List<HashSet<Tile>> MatchedTilesPerStep { get; } = new List<HashSet<Tile>>();

        public Prediction()
        {
            for (int i = 0; i < Simulator.MAX_SIMULATION_STEPS; ++i)
                MatchedTilesPerStep.Add(new HashSet<Tile>());
        }

        public Prediction(List<HashSet<Tile>> matchedTilesPerStep)
        {
            MatchedTilesPerStep = matchedTilesPerStep;
        }
    }
}
