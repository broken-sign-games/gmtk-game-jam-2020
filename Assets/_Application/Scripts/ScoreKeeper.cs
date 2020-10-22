using GMTK2020.Data;
using System;

namespace GMTK2020
{
    public class ScoreKeeper
    {
        public int TotalScore { get; private set; }

        private int baseScore;

        private int chainLength;

        public ScoreKeeper(int baseScore)
        {
            this.baseScore = baseScore;
            chainLength = 0;

            TotalScore = 0;
        }

        public int ScoreStep(SimulationStep step)
        {
            int stepScore = 0;
            switch (step)
            {
            case MatchStep matchStep: stepScore = ScoreMatchStep(matchStep); break;
            case CleanUpStep cleanUpStep: stepScore = ScoreCleanUpStep(cleanUpStep); break;
            }

            TotalScore += stepScore;

            return stepScore;
        }

        private int ScoreMatchStep(MatchStep matchStep)
        {
            ++chainLength;
            return matchStep.MatchedTiles.Count * baseScore * chainLength;
        }

        private int ScoreCleanUpStep(CleanUpStep _)
        {
            chainLength = 0;
            return 0;
        }
    }
}