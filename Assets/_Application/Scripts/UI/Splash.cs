using GMTK2020.Audio;
using GMTK2020.SceneManagement;
using GMTKJam2020.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GMTK2020.UI
{
    public class Splash : MonoBehaviour
    {
        private InputActions inputs;

        private void Awake()
        {
            inputs = new InputActions();

            inputs.Gameplay.Select.performed += OnSelect;
        }

        private void OnEnable()
        {
            inputs.Enable();
        }

        private void OnDisable()
        {
            inputs.Disable();
        }

        private void OnDestroy()
        {
            inputs.Gameplay.Select.performed -= OnSelect;
        }

        private void OnSelect(InputAction.CallbackContext obj)
        {
            SoundManager.Instance.PlayEffect(SoundEffect.Click);
            LoadLevelScene();
        }

        private void LoadLevelScene()
        {
            SceneLoader.Instance.LoadLevelScene();
        }
    }
}
