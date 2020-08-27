using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GMTK2020.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public enum Effect
        {
            CLICK,
            PREDICT,
            STEP_CORRECT,
            STEP_WRONG,
            WIN,
            YOU_WIN,
        }

        [SerializeField] private AudioClip[] clips = new AudioClip[0];

        private Dictionary<Effect, AudioClip> clipRepository;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            clipRepository = clips
                .ToDictionary(
                    clip => Utility.ParseEnum<Effect>(clip.name.Replace("-", "_"), true),
                    clip => clip);
        }

        public void PlayEffect(Effect effect, float pitchModifier = 1f)
        {
            audioSource.pitch = 1f + (pitchModifier * 0.1f);
            audioSource.PlayOneShot(clipRepository[effect]);
        }
    }

}