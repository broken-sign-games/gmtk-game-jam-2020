using GMTK2020.Audio;
using GMTK2020.SceneManagement;
using GMTK2020.TutorialSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class OptionsMenuActions : MonoBehaviour
    {
        [SerializeField] private Slider musicVolumeSlider = null;
        [SerializeField] private Slider soundEffectVolumeSlider = null;

        private PlayerPreferences playerPreferences;

        private void Awake()
        {
            playerPreferences = PlayerPreferences.Instance;
        }

        private void Start()
        {
            musicVolumeSlider.value = playerPreferences.MusicVolume;
            soundEffectVolumeSlider.value = playerPreferences.SoundEffectVolume;
        }

        public void ResetTutorial()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            TutorialManager.ResetTutorial();
        }

        public void UpdateMusicVolume(float volume)
        {
            playerPreferences.MusicVolume = volume;
        }

        public void UpdateSoundEffectVolume(float volume)
        {
            playerPreferences.SoundEffectVolume = volume;
        }

        public void GoToMainMenu()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            SceneLoader.Instance.LoadMainMenuScene();
        }
    } 
}
