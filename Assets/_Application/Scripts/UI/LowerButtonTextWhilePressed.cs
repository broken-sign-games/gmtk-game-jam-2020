using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(BetterButton))]
    public class LowerButtonTextWhilePressed : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI buttonText = null;
        [SerializeField] private float lowerBy = 5f;
        [SerializeField] private Color enabledColor = Color.white;
        [SerializeField] private Color disabledColor = Color.white;

        private Vector2 defaultPosition;
        private BetterButton button;

        private void Start()
        {
            button = GetComponent<BetterButton>();
            defaultPosition = buttonText.rectTransform.anchoredPosition;
        }

        private void Update()
        {
            buttonText.rectTransform.anchoredPosition = button.IsPressed || !button.interactable
                ? defaultPosition - new Vector2(0, lowerBy)
                : defaultPosition;

            buttonText.color = button.interactable
                ? enabledColor
                : disabledColor;
        }
    } 
}
