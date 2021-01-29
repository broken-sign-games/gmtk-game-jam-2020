using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(BetterButton))]
    public class LowerButtonTextWhilePressed : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI buttonText = null;
        [SerializeField] private float lowerBy = 5f;

        private Vector2 defaultPosition;
        private BetterButton button;

        private void Start()
        {
            button = GetComponent<BetterButton>();
            defaultPosition = buttonText.rectTransform.anchoredPosition;
        }

        private void Update()
        {
            buttonText.rectTransform.anchoredPosition = button.IsPressed
                ? defaultPosition - new Vector2(0, lowerBy)
                : defaultPosition;
        }
    } 
}
