using UnityEngine;

namespace GMTK2020
{
    public class OnlyShowOnDesktop : MonoBehaviour
    {
        private void Awake()
        {
            switch (Application.platform)
            {
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.LinuxPlayer:
                break;
            default:
                Destroy(gameObject);
                break;
            }
        }
    } 
}
