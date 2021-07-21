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

        private const int TURNS_PER_LEVEL = 5;
        private int turnCount = 0;

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
            return DOTween.Sequence();
        }
    }
}
