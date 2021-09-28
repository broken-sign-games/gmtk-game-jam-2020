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

        private float defaultPositionY;
        private BetterButton button;

        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            button = GetComponent<BetterButton>();
            defaultPositionY = rectTransform.anchoredPosition.y;
        }

        private void Update()
        {
            float targetY = button.IsPressed
                ? defaultPositionY - lowerBy
                : defaultPositionY;

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetY);

            buttonLabel.color = button.interactable
                ? enabledColor
                : disabledColor;
        }
    } 
}
