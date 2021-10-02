using DG.Tweening;
using GMTK2020.Audio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK2020.UI
{
    public class MenuSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu = null;
        [SerializeField] private GameObject optionsMenu = null;
        [SerializeField] private GameObject creditsMenu = null;
        [SerializeField] private CanvasGroup title = null;

        [SerializeField] private float mainMenuTitlePosition = 216;
        [SerializeField] private float creditsMenuTitlePosition = 300;

        [SerializeField] private float titleMoveDuration = 0.5f;
        [SerializeField] private float startUpDelay = 0.5f;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float pulseMagnitude = 1.1f;
        [SerializeField] private float delayFactor = 0.001f;

        private GameObject activeMenu;
        private Dictionary<GameObject, CanvasGroup[]> menuElements;

        private void Start()
        {
            menuElements = new Dictionary<GameObject, CanvasGroup[]>
            {
                [mainMenu] = mainMenu.GetComponentsInChildren<CanvasGroup>(true),
                [optionsMenu] = optionsMenu.GetComponentsInChildren<CanvasGroup>(true),
                [creditsMenu] = creditsMenu.GetComponentsInChildren<CanvasGroup>(true),
            };

            title.alpha = 0;

            foreach (CanvasGroup[] uiElements in menuElements.Values)
                foreach (CanvasGroup uiElement in uiElements)
                    uiElement.alpha = 0;

            DOTween.Sequence()
                .AppendInterval(startUpDelay)
                .AppendCallback(GoToMainMenu);
        }

        public void GoToMainMenu() => GoToMenu(mainMenu);
        public void GoToOptionsMenu() => GoToMenu(optionsMenu);
        public void GoToCreditsMenu() => GoToMenu(creditsMenu);

        private void GoToMenu(GameObject targetMenu)
        {
            Sequence seq = DOTween.Sequence();

            if (activeMenu)
            {
                // Hack! There should be a cleaner way to play this only when
                // we got here through a button press.
                SoundManager.Instance.PlayEffect(SoundEffect.Click);

                seq.Append(HideActiveMenu(targetMenu));
            }

            seq.Append(ShowNewMenu(targetMenu));

            activeMenu = targetMenu;
        }

        private Tween HideActiveMenu(GameObject targetMenu)
        {
            Sequence hideSeq = DOTween.Sequence();

            GameObject oldMenu = activeMenu;

            float topY = menuElements[oldMenu].Select(canvasGroup => canvasGroup.transform.position.y).Max();

            if (oldMenu != optionsMenu && targetMenu == optionsMenu)
            {
                topY = title.transform.position.y;
                hideSeq.Insert(0, title.transform.DOPunchScale(Vector3.one * pulseMagnitude, fadeDuration, 0, 0));
                hideSeq.Insert(fadeDuration / 2, title.DOFade(0, fadeDuration / 2));
                hideSeq.AppendCallback(() => title.DeactivateObject());
            }

            float baseLength = hideSeq.Duration();

            foreach (CanvasGroup uiElement in menuElements[oldMenu])
            {
                float delay = (topY - uiElement.transform.position.y) * delayFactor;
                hideSeq.Insert(baseLength + delay, uiElement.transform.DOPunchScale(Vector3.one * pulseMagnitude, fadeDuration, 0, 0));
                hideSeq.Insert(baseLength + delay + fadeDuration / 2, uiElement.DOFade(0, fadeDuration / 2));
            }

            hideSeq.AppendCallback(() => oldMenu.Deactivate());
            return hideSeq;
        }

        private Tween ShowNewMenu(GameObject targetMenu)
        {
            Sequence showSeq = DOTween.Sequence();

            float topY = menuElements[targetMenu].Select(canvasGroup => canvasGroup.transform.position.y).Max();

            if (targetMenu != optionsMenu)
            {
                var titleRectTransform = title.GetComponent<RectTransform>();
                float targetPosY = targetMenu == mainMenu ? mainMenuTitlePosition : creditsMenuTitlePosition;

                if (title.gameObject.activeSelf)
                {
                    showSeq.Append(titleRectTransform.DOAnchorPosY(targetPosY, titleMoveDuration));
                }
                else
                {
                    titleRectTransform.anchoredPosition = new Vector2(0, targetPosY);
                    topY = title.transform.position.y;
                    showSeq.AppendCallback(() => title.ActivateObject());
                    showSeq.Append(title.transform.DOPunchScale(Vector3.one * pulseMagnitude, fadeDuration, 0, 0));
                    showSeq.Join(title.DOFade(1, fadeDuration / 2));
                }
            }

            showSeq.AppendCallback(() => targetMenu.Activate());

            float baseLength = showSeq.Duration();

            foreach (CanvasGroup uiElement in menuElements[targetMenu])
            {
                float delay = (topY - uiElement.transform.position.y) * delayFactor;
                showSeq.Insert(baseLength + delay, uiElement.transform.DOPunchScale(Vector3.one * pulseMagnitude, fadeDuration, 0, 0));
                showSeq.Insert(baseLength + delay, uiElement.DOFade(1, fadeDuration / 2));
            }

            return showSeq;
        }
    } 
}
