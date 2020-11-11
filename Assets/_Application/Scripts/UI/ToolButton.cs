using GMTK2020.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    [RequireComponent(typeof(Button))]
    public class ToolButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI availableUsesText;
        [SerializeField] Tool tool;

        public Tool Tool => tool;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            availableUsesText.text = "0";
        }

        public void UpdateUses(int uses)
        {
            availableUsesText.text = uses.ToString();
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
