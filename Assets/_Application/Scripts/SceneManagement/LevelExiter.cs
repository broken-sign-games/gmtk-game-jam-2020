using GMTK2020.Audio;
using UnityEngine;

namespace GMTK2020.SceneManagement
{
    public class LevelExiter : MonoBehaviour
    {
        public void RestartLevel()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            SceneLoader.Instance.LoadScene(SceneID.Level);
        }
    }
}