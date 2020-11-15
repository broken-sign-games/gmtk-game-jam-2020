using RotaryHeart.Lib.SerializableDictionary;
using System;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class ToolDataMap : ScriptableObject
    {
        [Serializable]
        public class ToolData
        {
            public int InitialUses = 0;
            public MatchShape AwardedFor = MatchShape.None;
        }

        public SerializableDictionaryBase<Tool, ToolData> Map;

        public ToolDataMap()
        {
            Map = new SerializableDictionaryBase<Tool, ToolData>();

            foreach (var type in Utility.GetEnumValues<Tool>())
                Map[type] = new ToolData();
        }
    }
}