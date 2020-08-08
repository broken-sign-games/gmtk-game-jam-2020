using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;

namespace GMTK2020.Data
{
    public enum TutorialID
    {
        FirstMatch,
        SubsequentMatch,
        ChainRewards,
        StoneBlocks,
    }

    [CreateAssetMenu]
    public class TutorialMap : ScriptableObject
    {
        [Serializable]
        public class TutorialDict : SerializableDictionaryBase<TutorialID, Tutorial> { }

        public TutorialDict Map;

        public TutorialMap()
        {
            Map = new TutorialDict();

            foreach (var type in Utility.GetEnumValues<TutorialID>())
                Map[type] = null;
        }
    }
}
