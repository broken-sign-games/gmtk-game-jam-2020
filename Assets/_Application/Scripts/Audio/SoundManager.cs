using UnityEngine;
using GMTK2020.Data;
using Random = System.Random;

namespace GMTK2020.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] private SoundEffectData soundEffects = null;

        private AudioSource audioSource;

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

            audioSource = GetComponent<AudioSource>();

            rng = new Random(Time.frameCount);
        }

        public void PlayEffect(SoundEffect effect, float pitchModifier = 1f)
        {
            audioSource.pitch = 1f + (pitchModifier * 0.1f);

            AudioClip[] availableClips = soundEffects.Map[effect].Clips;

            audioSource.PlayOneShot(availableClips.RandomChoice(rng));
        }

        public void PlayEffectWithRandomPitch(SoundEffect effect)
        {
            PlayEffect(effect, (float)rng.NextDouble() * 2 - 1);
        }
    }

}