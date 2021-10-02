using DG.Tweening;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class LevelCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText = null;
        [SerializeField] private Image[] turnSegments = null;
        [SerializeField] private float inactiveAlpha = 0.25f;
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float segmentPulseMagnitude = 0.1f;
        [SerializeField] private float dissipateDuration = 0.5f;
        [SerializeField] private float dissipateScale = 1.2f;
        [SerializeField] private float levelPulseMagnitude = 0.2f;
        [SerializeField] private float levelPulseDuration = 0.25f;
        [SerializeField] private float reappearDuration = 0.5f;
        [SerializeField] private float reappearScale = 0.5f;
        [SerializeField] private float reappearOffset = 0.25f;

        private const int TURNS_PER_LEVEL = 5;
        private int turnCount = 0;
        private int currentLevel = 1;

        private void Start()
        {
            levelText.text = $"{currentLevel}";
        }

        public Tween IncrementTurns()
        {
            Sequence seq = DOTween.Sequence();

            Image segment = turnSegments[turnCount];
            seq.Append(segment.DOFade(1, fadeDuration));
            seq.Join(segment.transform.parent.DOPunchScale(Vector3.one * segmentPulseMagnitude, fadeDuration, 0, 0));

            ++turnCount;

            return seq;
        }

        public Tween IncrementLevel()
        {
            ++currentLevel;
            turnCount = 0;

            Sequence seq = DOTween.Sequence();

            foreach (Image segment in turnSegments)
            {
                seq.Join(segment.DOFade(0, dissipateDuration));
                seq.Join(segment.transform.parent.DOScale(dissipateScale, dissipateDuration));
            }

            seq.Join(levelText.transform.DOPunchScale(Vector3.one * levelPulseMagnitude, levelPulseDuration, 0, 0));
            seq.InsertCallback(levelPulseDuration / 2, () => levelText.text = $"{currentLevel}");

            for (int i = 0; i < turnSegments.Length; ++i)
            {
                Image segment = turnSegments[i];
                seq.InsertCallback(dissipateDuration, () => segment.transform.parent.localScale = Vector3.one * reappearScale);
                seq.Insert(dissipateDuration + i * reappearOffset, segment.DOFade(inactiveAlpha, reappearDuration));
                seq.Insert(dissipateDuration + i * reappearOffset, segment.transform.parent.DOScale(1, reappearDuration));
            }

            return seq;
        }
    }
}
