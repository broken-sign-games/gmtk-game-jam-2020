using GMTK2020.Data;
using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class MetricsRenderer : MonoBehaviour
    {
        [SerializeField] private SessionMetrics sessionMetrics;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI maxChainText;
        [SerializeField] private TextMeshProUGUI maxMatchText;
        [SerializeField] private TextMeshProUGUI mistakesText;
        [SerializeField] private TextMeshProUGUI maxResourceText;
        [SerializeField] private TextMeshProUGUI toolsGainedText;
        [SerializeField] private TextMeshProUGUI toolsUsedText;

        private void Update()
        {
            UpdateMetrics();
        }

        private void UpdateMetrics()
        {
            int seconds = Mathf.FloorToInt(sessionMetrics.SessionTime);
            timeText.text = $"{seconds / 60}:{seconds % 60:00}";
            maxChainText.text = sessionMetrics.MaxChainLength.ToString();
            maxMatchText.text = sessionMetrics.MaxMatchSize.ToString();
            mistakesText.text = sessionMetrics.MistakeCount.ToString();
            maxResourceText.text = sessionMetrics.MaxResource.ToString();
            toolsGainedText.text = $"–/{sessionMetrics.GetToolUnlocks(Tool.PlusBomb)}/{sessionMetrics.GetToolUnlocks(Tool.Rotate3x3)}/{sessionMetrics.GetToolUnlocks(Tool.RemoveTile)}/{sessionMetrics.GetToolUnlocks(Tool.RemoveRow)}/{sessionMetrics.GetToolUnlocks(Tool.CreateWildcard)}";
            toolsUsedText.text = $"{sessionMetrics.GetToolUses(Tool.SwapTiles)}/{sessionMetrics.GetToolUses(Tool.PlusBomb)}/{sessionMetrics.GetToolUses(Tool.Rotate3x3)}/{sessionMetrics.GetToolUses(Tool.RemoveTile)}/{sessionMetrics.GetToolUses(Tool.RemoveRow)}/{sessionMetrics.GetToolUses(Tool.CreateWildcard)}";
        }
    } 
}
