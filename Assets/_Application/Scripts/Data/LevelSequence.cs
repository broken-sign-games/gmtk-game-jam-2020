using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class LevelSequence : ScriptableObject
    {
        public List<LevelSpecification> Levels;
    }
}