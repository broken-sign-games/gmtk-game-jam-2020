using GMTK2020.Audio;
using GMTK2020.Rendering;
using GMTK2020.SceneManagement;
using UnityEngine;

namespace GMTK2020
{
    public class TutorialExiter : MonoBehaviour
    {
        [SerializeField] private string levelSceneName = null;
        [SerializeField] private BoardRenderer boardRenderer = null;

        public async void LoadLevel()
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>();
            if (soundManager)
                soundManager.PlayEffect(SoundManager.Effect.CLICK);
            boardRenderer.CancelAnimation();
            await new SceneLoader().LoadSceneAsync(levelSceneName);
        }
    }
}