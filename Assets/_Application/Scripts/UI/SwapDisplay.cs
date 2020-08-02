using TMPro;
using UnityEngine;

namespace GMTK2020.UI
{
    public class SwapDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI swapText = null;

        public void SetSwaps(int swaps)
        {
            swapText.text = swaps.ToString();
        }
    }
}