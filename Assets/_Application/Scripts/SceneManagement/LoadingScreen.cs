using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.SceneManagement
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image loadingBackground = null;
        [SerializeField] private float fadeDuration = 1f;

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