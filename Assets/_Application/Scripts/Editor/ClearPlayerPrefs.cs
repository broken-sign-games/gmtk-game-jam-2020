using UnityEngine;
using UnityEditor;

namespace GMTK2020.Editor
{
    public class ClearPlayerPrefs : MonoBehaviour
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        public static void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}