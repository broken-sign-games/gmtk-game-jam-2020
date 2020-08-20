using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class SceneLoader
    {
        private static Scene? activeScene;
        private static bool loading = false;

        public async void LoadScene(int sceneBuildIndex, GameObject parameterObject)
        {
            await LoadSceneAsync(sceneBuildIndex, parameterObject);
        }

        public async void LoadScene(string sceneName, GameObject parameterObject)
        {
            LoadScene(SceneUtility.GetBuildIndexByScenePath(sceneName), parameterObject);
        }

        public async Task LoadSceneAsync(int sceneBuildIndex, GameObject parameterObject)
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
        }
    }
}