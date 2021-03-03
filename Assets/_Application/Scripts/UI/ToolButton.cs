using DG.Tweening;
using GMTK2020.Audio;
using GMTK2020.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class ToolButton : MonoBehaviour
    {
        [SerializeField] private BoardManipulator boardManipulator = null;
        [SerializeField] private TextMeshProUGUI availableUsesText = null;
        [SerializeField] private Tool tool = Tool.SwapTiles;
        [SerializeField] private int availableUsesDefaultFontSize = 45;
        [SerializeField] private int availableUsesUnlimitedFontSize = 60;
        [SerializeField] private float moveUpBy = 30;
        [SerializeField] private float overshootBy = 10;
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private Sprite defaultSprite = null;
        [SerializeField] private Sprite activeSprite = null;

        protected BoardManipulator BoardManipulator => boardManipulator;

        public Tool Tool => tool;

        private RectTransform rectTransform;
        private Button button;
        private Image buttonImage;

        private float initialYPos;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            buttonImage = GetComponent<Image>();
            button = GetComponent<Button>();
            availableUsesText.text = "0";

            initialYPos = rectTransform.anchoredPosition.y;
        }

        public virtual void OnClick()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.ToolSelected);

            boardManipulator.ToggleTool(Tool);
        }

        public void UpdateUses(int uses)
        {
            if (uses < 0)
            {
                availableUsesText.text = "∞";
                availableUsesText.fontSize = availableUsesUnlimitedFontSize;
                availableUsesText.fontWeight = FontWeight.Bold;
            }
            else
            {
                availableUsesText.text = uses.ToString();
                availableUsesText.fontSize = availableUsesDefaultFontSize;
                availableUsesText.fontWeight = FontWeight.Regular;
            }
        }

        public void UpdateAvailable(bool available)
        {
            button.interactable = available;
        }

        public void UpdateChainLength(int awarded, int available, int chainLength)
        {
        }

        public void UpdateActive(bool active)
        {
            if (active)
            {
                rectTransform
                    .DOAnchorPosY(initialYPos + moveUpBy, moveDuration)
                    .SetEase(Ease.OutBack, overshoot: overshootBy);
            }
            else
            {
                rectTransform
                    .DOAnchorPosY(initialYPos, moveDuration);
            }

            buttonImage.sprite = active ? activeSprite : defaultSprite;
        }
    } 
}
