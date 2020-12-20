using System;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    [Serializable]
    public struct GridRect
    {
        [SerializeField] private Vector2Int _bottomLeft;
        [SerializeField] private Vector2Int _topRight;

        public Vector2Int BottomLeft => _bottomLeft;
        public Vector2Int TopRight => _topRight;

        public int Width => TopRight.x - BottomLeft.x + 1;
        public int Height => TopRight.y - BottomLeft.y + 1;

        public Vector2 Center => (Vector2)(BottomLeft + TopRight) / 2;

        public GridRect(Vector2Int corner1, Vector2Int corner2)
        {
            _bottomLeft = new Vector2Int(Math.Min(corner1.x, corner2.x), Math.Min(corner1.y, corner2.y));
            _topRight = new Vector2Int(Math.Max(corner1.x, corner2.x), Math.Max(corner1.y, corner2.y));
        }

        public bool IsInsideRect(Vector2Int pos)
            => BottomLeft.x <= pos.x 
            && pos.x <= TopRight.x 
            && BottomLeft.y <= pos.y 
            && pos.y <= TopRight.y;

        public IEnumerable<Vector2Int> GetPositions()
        {
            for (int y = BottomLeft.y; y <= TopRight.y; ++y)
                for (int x = BottomLeft.x; x <= TopRight.x; ++x)
                    yield return new Vector2Int(x, y);
        }
    }
}