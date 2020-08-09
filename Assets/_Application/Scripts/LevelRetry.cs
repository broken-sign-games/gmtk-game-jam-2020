using GMTK2020.Data;
using GMTK2020.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class LevelRetry : MonoBehaviour
    {
        [SerializeField] private LoadingPopup loadingPopup = null;
        [SerializeField] private string match3SceneName = null;

        private SoundManager soundManager = null;

        private void Start()
        {
            soundManager = FindObjectOfType<SoundManager>();
        }

        public void RestartLevel()
        {
            loadingPopup.Show();
            soundManager?.PlayEffect(SoundManager.Effect.CLICK);
            LoadMatch3Scene();
        }

        private void LoadMatch3Scene()
        {
            SceneManager.LoadScene(match3SceneName);
        }
    }
}