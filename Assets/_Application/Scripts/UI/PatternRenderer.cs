using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK2020.UI
{
    public class PatternRenderer : MonoBehaviour
    {
        [SerializeField] private Image tilePrefab = null;
        [SerializeField] private int tileSeparation = 7;

        public void RenderPattern(HashSet<Vector2Int> pattern)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (Vector2Int pos in pattern)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }

            Vector2 center = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);

            foreach (Vector2Int pos in pattern)
            {
                Image tile = Instantiate(tilePrefab, transform);
                tile.rectTransform.anchoredPosition = (pos - center) * tileSeparation;
            }
        }
    }
}