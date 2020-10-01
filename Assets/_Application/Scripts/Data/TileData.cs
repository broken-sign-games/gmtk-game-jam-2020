using GMTK2020.Rendering;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class TileData : ScriptableObject
    {
        public Sprite[] UnmarkedSpriteMap;
        public Sprite[] MarkedSpriteMap;
    }
}