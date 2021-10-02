using GMTK2020.TutorialSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(Button))]
    public class ResetTutorialButton : MonoBehaviour
    {
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.interactable = TutorialManager.GetGameCount() > 0;
        }

        public void Disable()
        {
            button.interactable = false;
        }
    } 
}
