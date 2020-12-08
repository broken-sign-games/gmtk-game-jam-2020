using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class LevelSpecification : ScriptableObject
    {
        public Vector2Int Size;
        public int InitialColorCount;
        public int MaxColorCount;
        public int ChainsUntilColorIsAdded;
        public int GuaranteedChain;
    }
}