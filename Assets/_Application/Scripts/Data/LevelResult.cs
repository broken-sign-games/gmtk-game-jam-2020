using System.Collections.Generic;

namespace GMTK2020.Data
{
    public struct LevelResult
    {
        public int CorrectPredictions { get; }
        public HashSet<Tile> MissingPredictions { get; }
        public HashSet<Tile> ExtraneousPredictions { get; }

        public LevelResult(int correctGuesses, HashSet<Tile> missingPredictions, HashSet<Tile> extraneousPredictions)
        {
            CorrectPredictions = correctGuesses;
            MissingPredictions = missingPredictions;
            ExtraneousPredictions = extraneousPredictions;
        }
    }
}