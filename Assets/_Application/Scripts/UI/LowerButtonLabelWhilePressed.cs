using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(BetterButton))]
    public class LowerButtonLabelWhilePressed : MonoBehaviour
    {
        [SerializeField] private Graphic buttonLabel = null;
        [SerializeField] private float lowerBy = 5f;
        [SerializeField] private Color enabledColor = Color.white;
        [SerializeField] private Color disabledColor = Color.white;

        private Vector2 defaultPosition;
        private BetterButton button;

        private void Start()
        {
            button = GetComponent<BetterButton>();
            defaultPosition = buttonLabel.rectTransform.anchoredPosition;
        }

        private void Update()
        {
            buttonLabel.rectTransform.anchoredPosition = button.IsPressed || !button.interactable
                ? defaultPosition - new Vector2(0, lowerBy)
                : defaultPosition;

            buttonLabel.color = button.interactable
                ? enabledColor
                : disabledColor;
        }
    } 
}
