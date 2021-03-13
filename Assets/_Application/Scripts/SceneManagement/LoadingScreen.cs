using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.SceneManagement
{
    [RequireComponent(typeof(Image))]
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 1f;

        private Image loadingBackground;

        private void Awake()
        {
            loadingBackground = GetComponent<Image>();
        }

        public Task Show()
        {
            var tcsReady = new TaskCompletionSource<bool>();

            loadingBackground
                .DOFade(1f, fadeDuration)
                .OnComplete(() => tcsReady.TrySetResult(true));

            return tcsReady.Task;
        }

        public Task Dismiss()
        {
            var tcsDone = new TaskCompletionSource<bool>();

            loadingBackground
                .DOFade(0f, fadeDuration)
                .OnComplete(() => tcsDone.TrySetResult(true));

            return tcsDone.Task;
        }
    }
}