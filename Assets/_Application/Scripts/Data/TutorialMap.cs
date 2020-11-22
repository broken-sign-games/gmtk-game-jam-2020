using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class TutorialMap : ScriptableObject
    {
        public SerializableDictionaryBase<TutorialID, Tutorial> Map;

        public TutorialMap()
        {
            Map = new SerializableDictionaryBase<TutorialID, Tutorial>();

            foreach (var type in Utility.GetEnumValues<TutorialID>())
                Map[type] = null;
        }
    }
}