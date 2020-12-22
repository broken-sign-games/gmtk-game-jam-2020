using Array2DEditor;
using System;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class FixedLevelStartData : ScriptableObject
    {
        [Serializable]
        public class FixedLevelStart
        {
            public Array2DInt BoardData;
            public Vector2Int[] CrackedTiles;
        }

        public FixedLevelStart[] Levels;
    } 
}
