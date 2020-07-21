using System.Collections.Generic;

namespace GMTK2020.Data
{
    public class Prediction
    {
        public HashSet<Tile> PredictedTiles { get; } = new HashSet<Tile>();

        public Prediction()
        {
        }

        public Prediction(HashSet<Tile> predictedTiles)
        {
            PredictedTiles = predictedTiles;
        }
    }
}
