using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class TutorialData : ScriptableObject
    {
        public List<Tutorial> Tutorials;

        public TutorialData()
        {
            Tutorials = new List<Tutorial>();

            foreach (var id in Utility.GetEnumValues<TutorialID>())
            {
                var tutorial = new Tutorial() { ID = id };
                Tutorials.Add(tutorial);
            }
        }
    }
}