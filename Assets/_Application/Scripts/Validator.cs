using GMTK2020.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace GMTK2020
{
    public class Validator
    {
        public LevelResult ValidatePrediction(Simulation simulation, Prediction prediction)
        {
            if (prediction.MatchedTilesPerStep.Count > simulation.Steps.Count)
                throw new ArgumentException("More predictions than simulation steps. This shouldn't have happened.");

            for (int i = 0; i < prediction.MatchedTilesPerStep.Count; ++i)
            {
                HashSet<Tile> predicted = prediction.MatchedTilesPerStep[i];
                HashSet<Tile> simulated = new HashSet<Tile>(simulation.Steps[i].MatchedTiles.Select((tuple) => {
                    (Tile tile, _) = tuple;
                    return tile;
                }));

                if (!predicted.SetEquals(simulated))
                {
                    var missing = new HashSet<Tile>(simulated.Except(predicted));
                    var extraneous = new HashSet<Tile>(predicted.Except(simulated));
                    return new LevelResult(i, missing, extraneous);
                }
            }

            return new LevelResult(
                prediction.MatchedTilesPerStep.Count,
                new HashSet<Tile>(),
                new HashSet<Tile>());
        }
    }
}
