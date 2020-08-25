using GMTK2020.Audio;
using GMTK2020.Rendering;
using UnityEngine;

namespace GMTK2020.SceneManagement
{
    public class TutorialExiter : MonoBehaviour
    {
        [SerializeField] private BoardRenderer boardRenderer = null;

        public void LoadLevel()
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>();
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.CLICK);
            boardRenderer.CancelAnimation();
            SceneLoader.Instance.LoadLevelScene();
        }
    }
}