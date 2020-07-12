using GMTK2020.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class LevelExiter : MonoBehaviour
    {
        [SerializeField] private string levelSceneName = null;
        [SerializeField] private string winSceneName = null;
        [SerializeField] private string tutorialSceneName = null;
        [SerializeField] private LevelSequence levelSequence = null;

        private SoundManager SoundManager = null;

        private void Start()
        {
            SoundManager = FindObjectOfType<SoundManager>();
        }

        public void RestartLevel()
        {
            LoadLevelScene();
        }

        public void LoadNextLevel()
        {
            ++GameProgression.CurrentLevelIndex;
            if (GameProgression.CurrentLevelIndex >= levelSequence.Levels.Count)
                LoadWinScene();
            else
            {
                if (levelSequence.Levels[GameProgression.CurrentLevelIndex].TutorialBoard != null)
                    LoadTutorialScene();
                else
                    LoadLevelScene();
            }
        }

        private void LoadLevelScene()
        {
            SoundManager?.PlayEffect(SoundManager.Effect.CLICK);
            SceneManager.LoadScene(levelSceneName);
        }

        private void LoadWinScene()
        {
            SceneManager.LoadScene(winSceneName);
        }

        private void LoadTutorialScene()
        {
            SceneManager.LoadScene(tutorialSceneName);
        }
    }
}