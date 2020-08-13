using System.Linq;
using UnityEngine;

namespace GMTK2020
{
    public class ToggleFullscreen : MonoBehaviour
    {
        private Resolution nativeResolution;

        private bool isFullscreen;
        private bool wasFullscreen;

        private void Start()
        {
            nativeResolution = Screen.resolutions.OrderByDescending(r => r.width).First();

            isFullscreen = wasFullscreen = Screen.fullScreen;

            DontDestroyOnLoad(gameObject);
        }

        private bool fixSize = false;
        private void Update()
        {
            isFullscreen = Screen.fullScreen;

            if (isFullscreen && !wasFullscreen)
            {
                Screen.SetResolution(nativeResolution.width, nativeResolution.height, FullScreenMode.FullScreenWindow);
            }
            else if (!isFullscreen && wasFullscreen)
            {
                Screen.SetResolution(960, 540, FullScreenMode.Windowed);
            }

            wasFullscreen = isFullscreen;
        }
    } 
}
