using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GMTK2020
{
    public class Validator
    {
        public int ValidatePrediction(Simulation simulation, Prediction prediction)
        {
            if (prediction.MatchedTilesPerStep.Count > simulation.Steps.Count)
                throw new ArgumentException("prediction");

            for (int i = 0; i < prediction.MatchedTilesPerStep.Count; i++)
            {
                var predicted = prediction.MatchedTilesPerStep[i];
                var simulated = simulation.Steps[i].MatchedTiles;

                if (!predicted.SetEquals(simulated))
                {
                    return i;
                }
            }

            return prediction.MatchedTilesPerStep.Count;
        }
    }
}
