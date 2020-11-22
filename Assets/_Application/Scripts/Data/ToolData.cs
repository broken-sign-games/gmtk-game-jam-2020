using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class ToolData : ScriptableObject
    {
        [Serializable]
        public class SingleToolData
        {
            public int InitialUses = 0;
            public MatchShape AwardedFor = MatchShape.None;
        }

        public RewardStrategy RewardStrategy;
        public SerializableDictionaryBase<Tool, SingleToolData> Map;

        public ToolData()
        {
            Map = new SerializableDictionaryBase<Tool, SingleToolData>();

            foreach (var type in Utility.GetEnumValues<Tool>())
                Map[type] = new SingleToolData();
        }
    }
}