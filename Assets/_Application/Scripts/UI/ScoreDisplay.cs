using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highscoreText;

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

        public void SetHighscore(int highscore)
        {
            highscoreText.text = highscore.ToString();
        }
    }
}