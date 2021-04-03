using DG.Tweening;
using GMTK2020.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.TutorialSystem
{
    public class TutorialGridMaskManager : MonoBehaviour
    {
        [SerializeField] private RectTransform reference00 = null;
        [SerializeField] private RectTransform reference11 = null;
        [SerializeField] private RectTransform tutorialMaskPrefab = null;
        [SerializeField] private Image tutorialOverlay = null;
        [SerializeField] private float overlayAlpha = 0.5f;
        [SerializeField] private float fadeDuration = 0.5f;

        private TutorialManager tutorialManager;

        private List<RectTransform> activeTutorialMasks;
        private Tutorial activeTutorial;

        private Vector2 gridCellSize;

        private void Awake()
        {
            activeTutorialMasks = new List<RectTransform>();

            tutorialManager = TutorialManager.Instance;

            tutorialManager.TutorialReady += OnTutorialReady;
            tutorialManager.TutorialCompleted += OnTutorialCompleted;
        }

        private void Start()
        {
            gridCellSize = reference11.localPosition - reference00.localPosition;
        }

        private void OnDestroy()
        {
            tutorialManager.TutorialReady -= OnTutorialReady;
            tutorialManager.TutorialCompleted -= OnTutorialCompleted;
        }

        private async Task OnTutorialReady(Tutorial tutorial)
        {
            activeTutorial = tutorial;

            foreach (GridRect rect in tutorial.InteractableRects)
            {
                // TODO: Bit of a hack, because overlapping rects are hard to
                // get to look nice.
                foreach (Vector2Int pos in rect.GetPositions())
                {
                    RectTransform mask = Instantiate(tutorialMaskPrefab, transform);
                    mask.sizeDelta = gridCellSize;
                    mask.localPosition = (Vector2)reference00.localPosition + pos * gridCellSize;

                    activeTutorialMasks.Add(mask);
                }
            }

            await tutorialOverlay.DOFade(overlayAlpha, fadeDuration).Completion();
        }

        private async Task OnTutorialCompleted(Tutorial tutorial)
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(tutorialOverlay.DOFade(0, fadeDuration));
            seq.AppendCallback(() =>
            {
                foreach (RectTransform mask in activeTutorialMasks)
                    Destroy(mask.gameObject);

                activeTutorialMasks.Clear();

                activeTutorial = null;
            });

            await seq.Completion();
        }

        public bool IsPositionAllowedByCurrentMask(Vector2Int pos)
        {
            if (activeTutorial is null)
                return true;

            if (activeTutorial.LockBoard)
                return false;

            foreach (GridRect rect in activeTutorial.InteractableRects)
                if (rect.IsInsideRect(pos))
                    return true;

            return false;
        }
    } 
}
