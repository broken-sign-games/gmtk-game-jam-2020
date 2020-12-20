using GMTK2020.Data;
using GMTK2020.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.TutorialSystem
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private PolygonCollider2D overlayCollider = null;
        [SerializeField] private TutorialMask tutorialMaskPrefab = null;
        [SerializeField] private ToolPanel toolPanel = null;

        private TutorialManager tutorialManager;

        private List<TutorialMask> activeTutorialMasks;
        private Tutorial activeTutorial;

        private Camera mainCamera;

        private void Awake()
        {
            activeTutorialMasks = new List<TutorialMask>();

            tutorialManager = TutorialManager.Instance;

            tutorialManager.TutorialReady += OnTutorialReady;
            tutorialManager.TutorialCompleted += OnTutorialCompleted;
        }

        private void Start()
        {
            mainCamera = Camera.main;
            FillScreen();
        }

        private void FillScreen()
        {
            float height = mainCamera.orthographicSize * 2;
            float width = mainCamera.aspect * height;

            overlayCollider.transform.localScale = new Vector2(width, height);
        }

        private void OnDestroy()
        {
            tutorialManager.TutorialReady -= OnTutorialReady;
            tutorialManager.TutorialCompleted -= OnTutorialCompleted;
        }

        private void OnTutorialReady(Tutorial tutorial)
        {
            activeTutorial = tutorial;

            UpdateToolMasks();

            overlayCollider.ActivateObject();

            foreach (GridRect rect in tutorial.InteractableRects)
            {
                TutorialMask mask = Instantiate(tutorialMaskPrefab, transform);
                mask.SetGridRect(rect);

                activeTutorialMasks.Add(mask);
            }
        }

        private void UpdateToolMasks()
        {
            overlayCollider.pathCount = 1 + activeTutorial.InteractableTools.Count;

            int pathID = 1;

            foreach (Tool tool in activeTutorial.InteractableTools)
            {
                Vector2[] corners = toolPanel.GetButtonCornersInWorldSpace(tool);

                Rect buttonRect = new Rect(corners[0], corners[2] - corners[0]);

                TutorialMask mask = Instantiate(tutorialMaskPrefab, transform);
                mask.SetWorldSpaceRect(buttonRect);

                activeTutorialMasks.Add(mask);

                for (int i = 0; i < corners.Length; ++i)
                    corners[i] /= overlayCollider.transform.localScale;
                overlayCollider.SetPath(pathID, corners);
                ++pathID;
            }
        }

        private void OnTutorialCompleted(Tutorial tutorial)
        {
            foreach (TutorialMask mask in activeTutorialMasks)
                Destroy(mask.gameObject);

            activeTutorialMasks.Clear();

            overlayCollider.DeactivateObject();
            overlayCollider.pathCount = 1;

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
