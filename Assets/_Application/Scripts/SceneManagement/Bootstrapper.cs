using UnityEngine;

namespace GMTK2020.SceneManagement
{
    public class Bootstrapper : MonoBehaviour
    {
        private async void Start()
        {
            await new SceneLoader().LoadSceneAsync("SplashScene");
            Destroy(gameObject);
        }
    } 
}
