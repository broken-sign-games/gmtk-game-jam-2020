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
        PREDICT_STEP_0,
        PREDICT_STEP_1,
        PREDICT_STEP_2,
        PREDICT_STEP_3,
        PREDICT_STEP_4,
        PREDICT_STEP_5,
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

    public void PlayEffect(Effect effect)
    {
        AudioSource.PlayOneShot(ClipRepository[effect]);
    }
}
