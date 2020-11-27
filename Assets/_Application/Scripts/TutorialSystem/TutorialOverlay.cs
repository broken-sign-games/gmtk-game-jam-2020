using GMTK2020.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.TutorialSystem
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer overlaySprite;
        [SerializeField] private TutorialMask tutorialMaskPrefab;

        private TutorialManager tutorialManager;

        private List<TutorialMask> activeTutorialMasks;
        private Tutorial activeTutorial;

        private void Awake()
        {
            activeTutorialMasks = new List<TutorialMask>();

            tutorialManager.TutorialReady += OnTutorialReady;
            tutorialManager.TutorialCompleted += OnTutorialCompleted;
        }

        private void OnDestroy()
        {
            tutorialManager.TutorialReady -= OnTutorialReady;
            tutorialManager.TutorialCompleted -= OnTutorialCompleted;
        }

        private void OnTutorialReady(Tutorial tutorial)
        {
            activeTutorial = tutorial;

            overlaySprite.ActivateObject();

            foreach (GridRect rect in tutorial.InteractableRects)
            {
                TutorialMask mask = Instantiate(tutorialMaskPrefab);
                mask.SetRect(rect);

                activeTutorialMasks.Add(mask);
            }
        }

        private void OnTutorialCompleted(Tutorial tutorial)
        {
            foreach (TutorialMask mask in activeTutorialMasks)
                Destroy(mask.gameObject);

            activeTutorialMasks.Clear();

            overlaySprite.DeactivateObject();

            activeTutorial = null;
        }

        public bool IsPositionAllowedByCurrentMask(Vector2Int pos)
        {
            if (activeTutorial is null)
                return true;

            foreach (GridRect rect in activeTutorial.InteractableRects)
                if (rect.IsInsideRect(pos))
                    return true;

            return false;
        }
    } 
}
