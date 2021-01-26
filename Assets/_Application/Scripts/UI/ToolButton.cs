using GMTK2020.Audio;
using GMTK2020.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(Button))]
    public class ToolButton : MonoBehaviour
    {
        [SerializeField] private BoardManipulator boardManipulator = null;
        [SerializeField] TextMeshProUGUI availableUsesText = null;
        [SerializeField] TextMeshProUGUI requiredChainLengthText = null;
        [SerializeField] Tool tool = Tool.SwapTiles;

        protected BoardManipulator BoardManipulator => boardManipulator;

        public Tool Tool => tool;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            availableUsesText.text = "0";
        }

        public virtual void OnClick()
        {
            SoundManager.Instance.PlayEffect(SoundEffect.ToolSelected);

            boardManipulator.ToggleTool(Tool);
        }

        public void UpdateUses(int uses)
        {
            availableUsesText.text = uses.ToString();
        }

        public void UpdateAvailable(bool available)
        {
            button.interactable = available;
        }

        public void UpdateChainLength(int awarded, int available, int chainLength)
        {
            
            requiredChainLengthText.text = $"{awarded}/{available}; x{chainLength}";
        }

        public void UpdateActive(bool active)
        {
            ColorBlock colors = button.colors;
            Color color = active ? Color.grey : Color.white;
            colors.normalColor = color;
            colors.selectedColor = color;
            button.colors = colors;
        }
    } 
}
