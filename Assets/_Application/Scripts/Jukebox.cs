using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class Jukebox : MonoBehaviour
{

    [SerializeField] private AudioClip[] Soundtrack = new AudioClip[0];

    private AudioSource AudioSource = null;
    private List<int> Playlist = null;
    private int CurrentPlaylistIndex = 0;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        Playlist = Soundtrack.Select((track, index) => (track, index))
            .OrderBy(it => System.Guid.NewGuid())
            .Select(track => track.index)
            .ToList();

        if (!AudioSource.playOnAwake)
        {
            PlayNextTrack();
        }
    }

    void Update()
    {
        if (!AudioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    private void PlayNextTrack()
    {
        CurrentPlaylistIndex = (CurrentPlaylistIndex + 1) % Playlist.Count;

        AudioSource.clip = Soundtrack[Playlist[CurrentPlaylistIndex]];
        AudioSource.Play();
    }

}
