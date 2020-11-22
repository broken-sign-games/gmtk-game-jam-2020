using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class LevelSpecification : ScriptableObject
    {
        public Vector2Int Size;
        public int ColorCount;
        public int GuaranteedChain;
    }
}