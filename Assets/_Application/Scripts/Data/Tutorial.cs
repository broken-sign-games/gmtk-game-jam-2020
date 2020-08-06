using Array2DEditor;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class Tutorial : ScriptableObject
    {
        [TextArea]
        public string Message;
        public Array2DInt Board;
        public List<Vector2Int> Predictions;
        public List<(Vector2Int, Vector2Int)> Swaps;
    }
}