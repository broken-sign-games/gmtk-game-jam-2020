using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{

    public enum Effect
    {
        CLICK,
        PREDICT,
    }

    [SerializeField] private AudioClip[] Clips;

    private Dictionary<Effect, AudioClip> ClipRepository = null;
    private AudioSource AudioSource = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        AudioSource = GetComponent<AudioSource>();
        ClipRepository = Clips.GroupBy(clip => clip.name)
            .ToDictionary(it => (Effect)Enum.Parse(typeof(Effect), it.Key.Replace("-", "_"), true), it => it.Single());
    }

    public void PlayEffect(Effect effect, float pitchModifier)
    {
        AudioSource.pitch = 1.0f + (pitchModifier * 0.1f);
        AudioSource.PlayOneShot(ClipRepository[effect]);
    }

    public void PlayEffect(Effect effect)
    {
        PlayEffect(effect, 1.0f);
    }
}
