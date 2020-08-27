using GMTK2020.Audio;
using GMTK2020.Data;
using UnityEngine;

namespace GMTK2020.SceneManagement
{
    public class LevelExiter : MonoBehaviour
    {
        [SerializeField] private LevelSequence levelSequence = null;

        private SoundManager soundManager;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();
        }

        public void RestartLevel()
        {
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.CLICK);
            SceneLoader.Instance.LoadLevelScene();
        }

        public void LoadNextLevel()
        {
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.CLICK);

            ++GameProgression.CurrentLevelIndex;
            if (GameProgression.CurrentLevelIndex >= levelSequence.Levels.Count)
                SceneLoader.Instance.LoadWinScene();
            else
            {
                if (levelSequence.Levels[GameProgression.CurrentLevelIndex].TutorialBoard != null)
                    SceneLoader.Instance.LoadTutorialScene();
                else
                    SceneLoader.Instance.LoadLevelScene();
            }
        }
    }
}