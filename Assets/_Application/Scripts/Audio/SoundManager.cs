using UnityEngine;
using GMTK2020.Data;
using Random = System.Random;

namespace GMTK2020.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public partial class SoundManager : MonoBehaviour
    {
        [SerializeField] private SoundEffectData soundEffects;

        private AudioSource audioSource;

        private Random rng;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            rng = new Random(Time.frameCount);
        }

        public void PlayEffect(SoundEffect effect, float pitchModifier = 1f)
        {
            audioSource.pitch = 1f + (pitchModifier * 0.1f);

            AudioClip[] availableClips = soundEffects.Map[effect].Clips;

            audioSource.PlayOneShot(availableClips.RandomChoice(rng));
        }
    }

}