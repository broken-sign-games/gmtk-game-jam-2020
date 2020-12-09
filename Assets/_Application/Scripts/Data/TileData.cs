using System;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class TileData : ScriptableObject
    {
        [Serializable]
        public struct VialSprites
        {
            public Sprite[] Sprites;

            public Sprite this[int i] => Sprites[i];
        }

        public VialSprites[] VialSpriteMap;
        public Sprite[] VialMaskMap;
        public Sprite[] LiquidSpriteMap;
        public Sprite[] CorkSpriteMap;
        public Sprite[] GlowSpriteMap;
        public Color[] GlowColor;
        public Color[] PopDropletColor;
    }
}