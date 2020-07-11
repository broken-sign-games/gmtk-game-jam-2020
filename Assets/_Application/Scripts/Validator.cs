using GMTK2020.Data;
using System;
using System.Collections.Generic;

namespace GMTK2020
{
    public class Validator
    {
        public int ValidatePrediction(Simulation simulation, Prediction prediction)
        {
            if (prediction.MatchedTilesPerStep.Count > simulation.Steps.Count)
                throw new ArgumentException("More predictions than simulation steps. This shouldn't have happened.");

            for (int i = 0; i < prediction.MatchedTilesPerStep.Count; ++i)
            {
                HashSet<Tile> predicted = prediction.MatchedTilesPerStep[i];
                HashSet<Tile> simulated = simulation.Steps[i].MatchedTiles;

                if (!predicted.SetEquals(simulated))
                {
                    return i;
                }
            }

            return prediction.MatchedTilesPerStep.Count;
        }
    }
}
