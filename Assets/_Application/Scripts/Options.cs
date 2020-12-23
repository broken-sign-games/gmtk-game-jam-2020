using GMTK2020.SceneManagement;
using GMTK2020.TutorialSystem;
using UnityEngine;

namespace GMTK2020
{
    public class Options : MonoBehaviour
    {
        public static readonly string SOUND_EFFECT_VOLUME_PREFS_KEY = "SoundEffectVolume";
        public static readonly string MUSIC_VOLUME_PREFS_KEY = "MusicVolume";

        public void ResetTutorial()
        {
            TutorialManager.ResetTutorial();
        }

        public void UpdateSoundEffectVolume(float volume)
        {
            PlayerPrefs.SetFloat(SOUND_EFFECT_VOLUME_PREFS_KEY, volume);
        }

        public void UpdateMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_PREFS_KEY, volume);
        }

        public void GoToMainMenu()
        {
            SceneLoader.Instance.LoadMainMenuScene();
        }
    } 
}
