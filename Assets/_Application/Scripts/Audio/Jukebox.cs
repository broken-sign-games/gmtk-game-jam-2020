using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;
using UnityEngine.Audio;

namespace GMTK2020.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class Jukebox : MonoBehaviour
    {
        [SerializeField] private AudioClip[] soundtrack = new AudioClip[0];
        [SerializeField] private AudioMixer mixer = null;

        private AudioSource audioSource;
        private List<int> playlist;
        private int currentPlaylistIndex = 0;

        private void Start()
        {
            PlayerPreferences playerPreferences = PlayerPreferences.Instance;

            UpdateMusicVolume(playerPreferences.MusicVolume);
            playerPreferences.MusicVolumeChanged += OnMusicVolumeChanged;
            
            Random rng = new Random();
            playlist = Enumerable.Range(0, soundtrack.Length).ToList().Shuffle(rng);

            audioSource = GetComponent<AudioSource>();

            if (!audioSource.playOnAwake)
            {
                PlayNextTrack();
            }
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                PlayNextTrack();
            }
        }

        private void OnMusicVolumeChanged(float volume)
        {
            UpdateMusicVolume(volume);
        }

        private void UpdateMusicVolume(float volume)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }

        private void PlayNextTrack()
        {
            ++currentPlaylistIndex;
            currentPlaylistIndex %= playlist.Count;

            audioSource.clip = soundtrack[playlist[currentPlaylistIndex]];
            audioSource.Play();
        }
    }
}