using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class ScoreRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText = null;
        [SerializeField] private Color newHighscoreColor = Color.green;

        private ScoreKeeper scoreKeeper;

        public void SetScoreKeeper(ScoreKeeper scoreKeeper)
        {
            this.scoreKeeper = scoreKeeper;

            UpdateScore();
        }

        public void UpdateScore()
        {
            scoreText.text = scoreKeeper.Score.ToString();

            if (scoreKeeper.Score > scoreKeeper.Highscore)
                scoreText.color = newHighscoreColor;
        }
    }
}