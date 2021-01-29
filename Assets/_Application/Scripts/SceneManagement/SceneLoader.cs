using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string loadingSceneName = "";
        [SerializeField] private string splashSceneName = "";
        [SerializeField] private string mainMenuSceneName = "";
        [SerializeField] private string levelSceneName = "";

        public static SceneLoader Instance { get; private set; }

        private int loadingSceneIndex;
        private Scene baseScene;
        private Scene? activeScene;
        private bool loading = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            loadingSceneIndex = SceneUtility.GetBuildIndexByScenePath(loadingSceneName);
            baseScene = SceneManager.GetSceneByBuildIndex(0);
        }

        public async void LoadSplashScene() => await LoadSceneAsync(splashSceneName);
        public async void LoadLevelScene() => await LoadSceneAsync(levelSceneName);
        public async void LoadMainMenuScene() => await LoadSceneAsync(mainMenuSceneName);

        private async Task LoadSceneAsync(string sceneName, GameObject parameterObject = null)
        {
            await LoadSceneAsync(SceneUtility.GetBuildIndexByScenePath(sceneName), parameterObject);
        }

        private async Task LoadSceneAsync(int sceneBuildIndex, GameObject parameterObject = null)
        {
            if (loading)
            {
                Debug.LogWarning("SceneLoader is already loading a scene. Ignoring LoadScene call.");
                return;
            }

            loading = true;

            await SceneManager.LoadSceneAsync(loadingSceneIndex, LoadSceneMode.Additive);

            LoadingScreen loadingScene = FindObjectOfType<LoadingScreen>();
            await loadingScene.Show();

            if (parameterObject)
                SceneManager.MoveGameObjectToScene(parameterObject, baseScene);

            await UnloadPreviousScene();

            await LoadNewScene(sceneBuildIndex);

            await Awaiters.NextFrame;

            await loadingScene.Dismiss();

            await SceneManager.UnloadSceneAsync(loadingScene.gameObject.scene);

            loading = false;
        }

        private async Task UnloadPreviousScene()
        {
            if (!activeScene.HasValue)
                return;
            
            await SceneManager.UnloadSceneAsync(activeScene.Value);
        }

        private async Task LoadNewScene(int sceneBuildIndex)
        {
            await SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

            activeScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
            SceneManager.SetActiveScene(activeScene.Value);
        }
    }
}