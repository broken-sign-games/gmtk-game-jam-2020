using TMPro;
using UnityEngine;

namespace GMTK2020.Rendering
{
    public class TileRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text = null;

        public void UpdatePrediction(int value)
        {
            text.text = value == 0 ? "" : value.ToString();
        }
    }
}