using GMTK2020.Data;
using System;
using UnityEngine;

namespace GMTK2020
{
    public class ScoreKeeper
    {
        public int Score { get; private set; }
        public int Highscore { get; private set; }

        private int baseScore;

        private int chainLength;

        private const string HIGHSCORE_KEY = "highscore";

        public ScoreKeeper(int baseScore)
        {
            this.baseScore = baseScore;
            chainLength = 0;

            Score = 0;
            LoadHighscore();
        }

        private void LoadHighscore()
        {
            if (PlayerPrefs.HasKey(HIGHSCORE_KEY))
                Highscore = PlayerPrefs.GetInt(HIGHSCORE_KEY);
            else
                Highscore = 0;
        }

        public int ScoreStep(SimulationStep step)
        {
            int stepScore = 0;
            switch (step)
            {
            case MatchStep matchStep: stepScore = ScoreMatchStep(matchStep); break;
            case CleanUpStep cleanUpStep: stepScore = ScoreCleanUpStep(cleanUpStep); break;
            }

            Score += stepScore;

            return stepScore;
        }

        public void UpdateHighscore()
        {
            if (Score > Highscore)
                Highscore = Score;

            PlayerPrefs.SetInt(HIGHSCORE_KEY, Highscore);
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