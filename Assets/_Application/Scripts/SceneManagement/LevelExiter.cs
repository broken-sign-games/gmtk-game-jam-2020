using GMTK2020.Audio;
using GMTK2020.Data;
using UnityEngine;

namespace GMTK2020.SceneManagement
{
    public class LevelExiter : MonoBehaviour
    {
        [SerializeField] private LevelSequence levelSequence = null;

        public void RestartLevel()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            SceneLoader.Instance.LoadLevelScene();
        }

        public void LoadNextLevel()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);

            ++GameProgression.CurrentLevelIndex;
            if (GameProgression.CurrentLevelIndex >= levelSequence.Levels.Count)
                SceneLoader.Instance.LoadWinScene();
            else
            {
                SceneLoader.Instance.LoadLevelScene();
            }
        }
    }
}