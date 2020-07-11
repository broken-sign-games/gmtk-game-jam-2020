using Array2DEditor;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class LevelSpecification : ScriptableObject
    {
        public Vector2Int Size;
        public int ColorCount;
        public List<Vector2Int> MatchingPattern;
        public GeneratorStrategy GeneratorStrategy;

        [TextArea]
        public string TutorialText;
        public Array2DInt TutorialBoard;
    }
}