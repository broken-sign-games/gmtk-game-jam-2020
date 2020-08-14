using GMTK2020.Audio;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    private SoundManager soundManager;

    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager)
            soundManager.PlayEffect(SoundManager.Effect.YOU_WIN);
    }
}
