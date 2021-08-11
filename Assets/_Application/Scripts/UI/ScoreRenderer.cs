using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class ScoreRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText = null;
        [SerializeField] private TextMeshProUGUI highscoreLabel = null;
        [SerializeField] private TextMeshProUGUI highscoreText = null;
        [SerializeField] private Color newHighscoreColor = Color.green;
        [SerializeField] private string defaultHighscoreLabel = "Highscore";
        [SerializeField] private string newHighscoreLabel = "New Highscore!";

        private ScoreKeeper scoreKeeper;

        private void Start()
        {
            highscoreLabel.text = defaultHighscoreLabel;
        }

        public void SetScoreKeeper(ScoreKeeper scoreKeeper)
        {
            this.scoreKeeper = scoreKeeper;

            UpdateScore();
        }

        public void UpdateScore()
        {
            scoreText.text = scoreKeeper.Score.ToString();

            if (scoreKeeper.Score > scoreKeeper.Highscore)
            {
                scoreText.color = newHighscoreColor;
                highscoreLabel.text = newHighscoreLabel;
            }
        }

        public void UpdateHighscore()
        {
            highscoreText.text = scoreKeeper.Highscore.ToString();
        }
    }
}