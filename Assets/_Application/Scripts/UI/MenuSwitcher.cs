using DG.Tweening;
using GMTK2020.Audio;
using UnityEngine;

namespace GMTK2020.UI
{
    public class MenuSwitcher : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainMenu = null;
        [SerializeField] private CanvasGroup optionsMenu = null;
        [SerializeField] private CanvasGroup creditsMenu = null;

        [SerializeField] private float fadeDuration = 1f;

        private CanvasGroup activeMenu;

        private void Start()
        {
            GoToMainMenu();
        }

        public void GoToMainMenu() => GoToMenu(mainMenu);
        public void GoToOptionsMenu() => GoToMenu(optionsMenu);
        public void GoToCreditsMenu() => GoToMenu(creditsMenu);

        private void GoToMenu(CanvasGroup targetMenu)
        {
            Sequence seq = DOTween.Sequence();

            if (activeMenu)
            {
                // Hack! There should be a cleaner way to play this only when
                // we got here through a button press.
                SoundManager.Instance.PlayEffect(SoundEffect.Click);

                CanvasGroup oldMenu = activeMenu;

                seq.Append(oldMenu.DOFade(0, fadeDuration));
                seq.AppendCallback(() => oldMenu.DeactivateObject());
            }

            seq.AppendCallback(() => targetMenu.ActivateObject());
            seq.Append(targetMenu.DOFade(1, fadeDuration));

            activeMenu = targetMenu;
        }
    } 
}
