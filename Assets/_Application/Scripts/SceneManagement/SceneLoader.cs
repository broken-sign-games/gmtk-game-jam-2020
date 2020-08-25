using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string splashSceneName = "";
        [SerializeField] private string tutorialSceneName = "";
        [SerializeField] private string levelSceneName = "";
        [SerializeField] private string winSceneName = "";

        public static SceneLoader Instance { get; private set; }

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
        }

        public async void LoadSplashScene() => await LoadSceneAsync(splashSceneName);
        public async void LoadLevelScene() => await LoadSceneAsync(levelSceneName);
        public async void LoadWinScene() => await LoadSceneAsync(winSceneName);
        public async void LoadTutorialScene() => await LoadSceneAsync(tutorialSceneName);

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

            await UnloadPreviousScene(parameterObject);

            await LoadNewScene(sceneBuildIndex);

            loading = false;
        }

        private async Task UnloadPreviousScene(GameObject parameterObject)
        {
            if (activeScene.HasValue)
            {
                Scene baseScene = SceneManager.GetSceneByBuildIndex(0);
                if (parameterObject)
                    SceneManager.MoveGameObjectToScene(parameterObject, baseScene);

                await SceneManager.UnloadSceneAsync(activeScene.Value);
            }
        }

        private async Task LoadNewScene(int sceneBuildIndex)
        {
            await SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

            activeScene = SceneManager.GetSceneByBuildIndex(sceneBuildIndex);
            SceneManager.SetActiveScene(activeScene.Value);
        }
    }
}