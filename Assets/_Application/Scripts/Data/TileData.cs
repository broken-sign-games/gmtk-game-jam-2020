using GMTK2020.Rendering;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class TileData : ScriptableObject
    {
        public Sprite[] VialSpriteMap;
        public Sprite[] LiquidSpriteMap;
        public Sprite[] CorkSpriteMap;
        public Sprite[] GlowSpriteMap;
        public Color[] GlowColor;
        public Color[] PopDropletColor;
    }
}