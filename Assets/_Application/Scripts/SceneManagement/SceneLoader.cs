using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SerializableDictionaryBase<SceneID, SceneReference> portraitScenes = null;
        [SerializeField] private SerializableDictionaryBase<SceneID, SceneReference> landscapeScenes = null;
        [SerializeField] private LoadingScreen loadingScreen = null;

        public static SceneLoader Instance { get; private set; }

        private Scene baseScene;
        private Scene? activeScene;
        private bool loading = false;

        private IDictionary<SceneID, SceneReference> _activeSceneMap;
        private IDictionary<SceneID, SceneReference> ActiveSceneMap
        {
            get
            {
                if (_activeSceneMap is null)
                    _activeSceneMap = Screen.width >= Screen.height ? landscapeScenes : portraitScenes;

                return _activeSceneMap;
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            baseScene = SceneManager.GetSceneByBuildIndex(0);
        }

        public async void LoadScene(SceneID sceneID) 
            => await LoadSceneAsync(sceneID);

        private async Task LoadSceneAsync(SceneID sceneID, GameObject parameterObject = null)
        {
            if (loading)
            {
                Debug.LogWarning("SceneLoader is already loading a scene. Ignoring LoadScene call.");
                return;
            }

            loading = true;

            SceneReference targetScene = ActiveSceneMap[sceneID];

            await loadingScreen.Show();

            if (parameterObject)
                SceneManager.MoveGameObjectToScene(parameterObject, baseScene);

            await UnloadPreviousScene();
            await LoadNewScene(targetScene);

            await Awaiters.NextFrame;

            loading = false;

            await loadingScreen.Dismiss();
        }

        private async Task UnloadPreviousScene()
        {
            if (!activeScene.HasValue)
                return;
            
            await SceneManager.UnloadSceneAsync(activeScene.Value);
        }

        private async Task LoadNewScene(SceneReference scene)
        {
            await SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

            activeScene = SceneManager.GetSceneByPath(scene);
            SceneManager.SetActiveScene(activeScene.Value);
        }
    }
}