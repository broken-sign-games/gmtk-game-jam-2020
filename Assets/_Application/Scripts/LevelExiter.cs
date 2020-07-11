using GMTK2020.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class LevelExiter : MonoBehaviour
    {
        [SerializeField] private int levelSceneID;
        [SerializeField] private int winSceneID;
        [SerializeField] private LevelSequence levelSequence;

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
                LoadLevelScene();
        }

        private void LoadLevelScene()
        {
            SceneManager.LoadScene(levelSceneID);
        }

        private void LoadWinScene()
        {
            SceneManager.LoadScene(winSceneID);
        }
    }
}