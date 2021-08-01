using DG.Tweening;
using GMTK2020.Audio;
using GMTK2020.SceneManagement;
using GMTK2020.TutorialSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class InGameMenus : MonoBehaviour
    {
        [SerializeField] private Image menuOverlay = null;
        [SerializeField] private RectTransform gameMenu = null;
        [SerializeField] private RectTransform boardTransform = null;
        [SerializeField] private BoardManipulator boardManipulator = null;

        [SerializeField] private float overlayAlpha = 0.5f;
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float slideDuration = 0.5f;
        [SerializeField] private float slideOvershoot = 1.25f;

        private Vector2 offScreenGameMenuPosition;

        private void Awake()
        {
            menuOverlay.SetAlpha(0);

            offScreenGameMenuPosition = gameMenu.anchoredPosition;

            foreach (var button in gameMenu.GetComponentsInChildren<Button>())
                button.enabled = false;
        }

        public void ShowGameMenu()
        {
            menuOverlay.raycastTarget = true;
            boardManipulator.enabled = false;

            foreach (var button in gameMenu.GetComponentsInChildren<Button>())
                button.enabled = true;

            Sequence seq = DOTween.Sequence();
            seq.Join(menuOverlay.DOFade(overlayAlpha, fadeDuration));
            seq.Join(gameMenu.DOMove(boardTransform.position, slideDuration).SetEase(Ease.OutBack, slideOvershoot));
        }

        public void DismissGameMenu()
        {
            foreach (var button in gameMenu.GetComponentsInChildren<Button>())
                button.enabled = false;

            Sequence seq = DOTween.Sequence();
            seq.Join(menuOverlay.DOFade(0, fadeDuration));
            seq.Join(gameMenu.DOAnchorPos(offScreenGameMenuPosition, slideDuration).SetEase(Ease.InBack, slideOvershoot));
            seq.AppendCallback(() => {
                menuOverlay.raycastTarget = false;
                boardManipulator.enabled = true;
            });
        }

        public void GoToMainMenu()
        {
            TutorialManager.Instance.CompleteActiveTutorial();
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            SceneLoader.Instance.LoadScene(SceneID.Menu);
        }
    } 
}
