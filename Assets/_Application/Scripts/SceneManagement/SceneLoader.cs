using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class SceneLoader
    {
        private static Scene? activeScene;
        private static bool loading = false;

        public async Task LoadSceneAsync(string sceneName, GameObject parameterObject = null)
        {
            await LoadSceneAsync(SceneUtility.GetBuildIndexByScenePath(sceneName), parameterObject);
        }

        public async Task LoadSceneAsync(int sceneBuildIndex, GameObject parameterObject = null)
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