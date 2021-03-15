using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(BetterButton))]
    public class LowerButtonWhilePressed : MonoBehaviour
    {
        [SerializeField] private Graphic buttonLabel = null;
        [SerializeField] private float lowerBy = 5f;
        [SerializeField] private Color enabledColor = Color.white;
        [SerializeField] private Color disabledColor = Color.white;

        private Vector2 defaultPosition;
        private BetterButton button;

        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            button = GetComponent<BetterButton>();
            defaultPosition = rectTransform.anchoredPosition;
        }

        private void Update()
        {
            rectTransform.anchoredPosition = button.IsPressed
                ? defaultPosition - new Vector2(0, lowerBy)
                : defaultPosition;

            buttonLabel.color = button.interactable
                ? enabledColor
                : disabledColor;
        }
    } 
}
