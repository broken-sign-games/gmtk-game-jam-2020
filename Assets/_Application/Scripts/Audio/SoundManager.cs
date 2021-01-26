using UnityEngine;
using GMTK2020.Data;
using Random = System.Random;
using UnityEngine.Audio;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;

namespace GMTK2020.Audio
{
    public class SoundManager : MonoBehaviour
    {
        private struct QueuedSoundEffect
        {
            public SoundEffect SoundEffect;
            public AudioSource AudioSource;
            public bool OneShot;
        }

        public static SoundManager Instance { get; private set; }

        [SerializeField] private SoundEffectData soundEffects = null;
        [SerializeField] private AudioMixer mixer = null;
        [SerializeField] private SerializableDictionaryBase<SoundEffect, AudioSource> audioSources = null;

        private Queue<QueuedSoundEffect> soundEffectQueue = new Queue<QueuedSoundEffect>();

        private Random rng;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Two instances of SoundManager detected. Deleting this one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            rng = new Random(Time.frameCount);
        }

        private void Start()
        {
            PlayerPreferences playerPreferences = PlayerPreferences.Instance;

            UpdateFXVolume(playerPreferences.SoundEffectVolume);

            playerPreferences.SoundEffectVolumeChanged += OnSoundEffectVolumeChanged;
        }

        private void Update()
        {
            if (soundEffectQueue.Count > 0)
                PlayQueuedEffect(soundEffectQueue.Dequeue());
        }

        public void PlayEffect(SoundEffect effect)
        {
            soundEffectQueue.Enqueue(new QueuedSoundEffect
            {
                SoundEffect = effect,
                AudioSource = audioSources[effect],
                OneShot = true,
            });
        }

        public void StartPlayingLoopEffect(AudioSource source, SoundEffect effect)
        {
            soundEffectQueue.Enqueue(new QueuedSoundEffect
            {
                SoundEffect = effect,
                AudioSource = source,
                OneShot = false,
            });
        }

        private void OnSoundEffectVolumeChanged(float volume)
        {
            UpdateFXVolume(volume);
        }

        private void UpdateFXVolume(float volume)
        {
            mixer.SetFloat("FXVolume", Mathf.Log10(volume) * 20);
        }

        private void PlayQueuedEffect(QueuedSoundEffect effect)
        {
            AudioClip[] availableClips = soundEffects.Map[effect.SoundEffect].Clips;
            AudioClip clip = availableClips.RandomChoice(rng);

            if (effect.OneShot)
                effect.AudioSource.PlayOneShot(clip);
            else
            {
                effect.AudioSource.clip = clip;
                effect.AudioSource.Play();
            }
        }

        public void StopEffect(AudioSource source)
        {
            source.Stop();
        }
    }

}