using DG.Tweening;
using GMTK2020.Audio;
using GMTK2020.Data;
using GMTK2020.TutorialSystem;
using System.Threading.Tasks;
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
        [SerializeField] private ToolData toolData = null;
        [SerializeField] private Image mask = null;
        [SerializeField] private Image toolIcon = null;
        [SerializeField] private Image patternIcon = null;
        [SerializeField] private Tool tool = Tool.SwapTiles;
        [SerializeField] private int availableUsesDefaultFontSize = 45;
        [SerializeField] private int availableUsesUnlimitedFontSize = 60;
        [SerializeField] private float moveUpBy = 30;
        [SerializeField] private float moveUsedDownBy = 90;
        [SerializeField] private float overshootBy = 10;
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private float pulseFactor = 1.2f;
        [SerializeField] private float pulseDuration = 0.25f;
        [SerializeField] private Sprite defaultSprite = null;
        [SerializeField] private Sprite activeSprite = null;
        [SerializeField] private Color enabledPatternColor = Color.white;
        [SerializeField] private Color disabledPatternColor = Color.white;

        protected Image ToolIcon => toolIcon;

        protected BoardManipulator BoardManipulator => boardManipulator;

        public Tool Tool => tool;

        private RectTransform rectTransform;
        private Button button;
        private Image buttonImage;

        private float initialYPos;
        private TutorialManager tutorialManager;

        private bool available;
        protected bool Available => available;

        private int? currentUses;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            buttonImage = GetComponent<Image>();
            button = GetComponent<Button>();
            availableUsesText.text = "0";

            initialYPos = rectTransform.anchoredPosition.y;

            toolIcon.sprite = toolData.Map[tool].EnabledSprite;

            tutorialManager = TutorialManager.Instance;
            tutorialManager.TutorialReady += OnTutorialReady;
            tutorialManager.TutorialCompleted += OnTutorialCompleted;
        }

        private void OnDestroy()
        {
            tutorialManager.TutorialReady -= OnTutorialReady;
            tutorialManager.TutorialCompleted -= OnTutorialCompleted;
        }

        public virtual void OnClick()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.ToolSelected);

            boardManipulator.ToggleTool(Tool);
        }

        public void UpdateRemainingUses(int uses)
        {
            if (uses < 0)
            {
                availableUsesText.text = "∞";
                availableUsesText.fontSize = availableUsesUnlimitedFontSize;
                availableUsesText.fontWeight = FontWeight.Bold;
            }
            else if (!currentUses.HasValue)
            {
                availableUsesText.text = uses.ToString();
                availableUsesText.fontSize = availableUsesDefaultFontSize;
                availableUsesText.fontWeight = FontWeight.Regular;
            }
            else if (currentUses < uses)
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(availableUsesText.DOFontSize(availableUsesDefaultFontSize * pulseFactor, pulseDuration).SetEase(Ease.Linear));
                seq.AppendCallback(() => availableUsesText.text = uses.ToString());
                seq.Append(availableUsesText.DOFontSize(availableUsesDefaultFontSize, pulseDuration).SetEase(Ease.Linear));
            }
            else
            {
                availableUsesText.text = uses.ToString();
            }

            currentUses = uses;
        }

        public virtual void UpdateAvailable(bool available)
        {
            this.available = available;
            button.interactable = available;

            toolIcon.sprite = available
                ? toolData.Map[tool].EnabledSprite
                : toolData.Map[tool].DisabledSprite;

            patternIcon.color = available
                ? enabledPatternColor
                : disabledPatternColor;

            availableUsesText.color = available
                ? enabledPatternColor
                : disabledPatternColor;
        }

        public void UpdateChainLength(int awarded, int available, int chainLength)
        {
        }

        public void UpdateActive(bool active, bool used)
        {
            if (active)
            {
                rectTransform
                    .DOAnchorPosY(initialYPos + moveUpBy, moveDuration)
                    .SetEase(Ease.OutBack, overshoot: overshootBy);
            }
            else if (used)
            {
                rectTransform
                    .DOAnchorPosY(initialYPos - moveUsedDownBy, moveDuration * 1.5f);
            }
            else
            {
                rectTransform
                    .DOAnchorPosY(initialYPos, moveDuration);
            }

            buttonImage.sprite = active ? activeSprite : defaultSprite;
        }

        private Task OnTutorialReady(Tutorial tutorial)
        {
            if (tutorial.InteractableTools.Contains(Tool))
                mask.enabled = true;
            else
                button.interactable = false;

            return Task.CompletedTask;
        }

        private Task OnTutorialCompleted(Tutorial tutorial)
        {
            mask.enabled = false;
            button.interactable = available;

            return Task.CompletedTask;
        }
    } 
}
