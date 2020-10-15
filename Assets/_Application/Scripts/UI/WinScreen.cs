using GMTK2020.Audio;
using UnityEngine;

namespace GMTK2020
{
    public class WinScreen : MonoBehaviour
    {
        private SoundManager soundManager;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();
            if (soundManager)
                soundManager.PlayEffect(SoundEffect.YouWin);
        }
    } 
}
