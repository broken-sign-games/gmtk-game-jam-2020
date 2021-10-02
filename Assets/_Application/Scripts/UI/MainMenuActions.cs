using GMTK2020.Audio;
using GMTK2020.SceneManagement;
using UnityEngine;

namespace GMTK2020.UI
{
    public class MainMenuActions : MonoBehaviour
    {
        public void StartGame()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            SceneLoader.Instance.LoadScene(SceneID.Level);
        }
    } 
}
