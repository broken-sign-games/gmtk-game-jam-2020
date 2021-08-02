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

        private const string HIGHSCORE_KEY = "highscore";

        public ScoreKeeper(int baseScore)
        {
            this.baseScore = baseScore;

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

        public int ScoreStep(SimulationStep step, int level)
        {
            int stepScore = 0;
            switch (step)
            {
            case MatchStep matchStep: stepScore = ScoreMatchStep(matchStep, level); break;
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

        private int ScoreMatchStep(MatchStep matchStep, int level)
        {
            return matchStep.MatchedTiles.Count * baseScore * matchStep.ChainLength * (level + 1);
        }

        private int ScoreCleanUpStep(CleanUpStep _)
        {
            return 0;
        }
    }
}