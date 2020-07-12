using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    private SoundManager SoundManager = null;

    void Start()
    {
        SoundManager = FindObjectOfType<SoundManager>();
        SoundManager?.PlayEffect(SoundManager.Effect.YOU_WIN);
    }
}
