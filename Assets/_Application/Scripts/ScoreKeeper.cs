using GMTK2020.Data;
using System;
using UnityEngine;

namespace GMTK2020
{
    public class ScoreKeeper
    {
        public int Score { get; private set; }
        public int Highscore { get; private set; }

        private const string HIGHSCORE_KEY = "highscore";

        public ScoreKeeper()
        {
            ResetScore();
            LoadHighscore();
        }

        private void LoadHighscore()
        {
            if (PlayerPrefs.HasKey(HIGHSCORE_KEY))
                Highscore = PlayerPrefs.GetInt(HIGHSCORE_KEY);
            else
                Highscore = 0;
        }

        public void ResetScore()
        {
            Score = 0;
        }

        public int ScoreStep(SimulationStep step, int stepIndex)
        {
            int stepScore = step.MatchedTiles.Count * (stepIndex + 1) * 100;
            Score += stepScore;
            return Score;
        }

        public void UpdateHighscore()
        {
            if (Score > Highscore)
                Highscore = Score;

            PlayerPrefs.SetInt(HIGHSCORE_KEY, Highscore);
        }
    }
}