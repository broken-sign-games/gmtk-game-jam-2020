using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class ScoreRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText = null;

        public void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}