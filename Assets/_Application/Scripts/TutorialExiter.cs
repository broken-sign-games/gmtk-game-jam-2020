using GMTK2020.Audio;
using GMTK2020.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMTK2020
{
    public class TutorialExiter : MonoBehaviour
    {
        [SerializeField] private string levelSceneName = null;
        [SerializeField] private BoardRenderer boardRenderer = null;

        public void LoadLevel()
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>();
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.CLICK);
            boardRenderer.CancelAnimation();
            SceneManager.LoadScene(levelSceneName);
        }
    }
}