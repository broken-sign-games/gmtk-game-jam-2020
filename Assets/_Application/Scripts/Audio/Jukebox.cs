using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
using Random = System.Random;

namespace GMTK2020.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class Jukebox : MonoBehaviour
    {
        [FormerlySerializedAs("Soundtrack")]
        [SerializeField] private AudioClip[] soundtrack = new AudioClip[0];

        private AudioSource audioSource;
        private List<int> playlist;
        private int currentPlaylistIndex = 0;

        private void Start()
        {
            PlayerPreferences playerPreferences = PlayerPreferences.Instance;

            audioSource = GetComponent<AudioSource>();
            audioSource.volume = playerPreferences.MusicVolume;

            playerPreferences.MusicVolumeChanged += OnMusicVolumeChanged;
            
            Random rng = new Random();
            playlist = Enumerable.Range(0, soundtrack.Length).ToList().Shuffle(rng);

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
            audioSource.volume = volume;
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