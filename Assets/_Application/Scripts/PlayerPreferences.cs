using System;
using UnityEngine;

namespace GMTK2020
{
    public class PlayerPreferences : MonoBehaviour
    {
        public static PlayerPreferences Instance { get; private set; }

        private static readonly string MUSIC_VOLUME_PREFS_KEY = "MusicVolume";
        private static readonly string SOUND_EFFECT_VOLUME_PREFS_KEY = "SoundEffectVolume";

        private const float DEFAULT_VOLUME = 0.8f;

        public event Action<float> MusicVolumeChanged;
        public event Action<float> SoundEffectVolumeChanged;

        public float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MUSIC_VOLUME_PREFS_KEY, DEFAULT_VOLUME);
            set
            {
                PlayerPrefs.SetFloat(MUSIC_VOLUME_PREFS_KEY, Mathf.Clamp01(value));
                MusicVolumeChanged?.Invoke(value);
            }
        }

        public float SoundEffectVolume
        {
            get => PlayerPrefs.GetFloat(SOUND_EFFECT_VOLUME_PREFS_KEY, DEFAULT_VOLUME);
            set
            {
                PlayerPrefs.SetFloat(SOUND_EFFECT_VOLUME_PREFS_KEY, Mathf.Clamp01(value));
                SoundEffectVolumeChanged?.Invoke(value);
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Two instances of PlayerPreferences detected. Deleting this one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}