using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Jukebox : MonoBehaviour
{

    [SerializeField] private AudioClip[] Soundtrack;

    private AudioSource AudioSource = null;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        this.AudioSource = GetComponent<AudioSource>();

        if (!AudioSource.playOnAwake)
        {
            AudioSource.clip = Soundtrack[Random.Range(0, Soundtrack.Length)];
            AudioSource.Play();
        }
    }

    void Update()
    {
        if (!AudioSource.isPlaying)
        {
            AudioSource.clip = Soundtrack[Random.Range(0, Soundtrack.Length)];
            AudioSource.Play();
        }
    }

}
