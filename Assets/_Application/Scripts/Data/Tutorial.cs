using UnityEngine;

namespace GMTK2020.Data
{
    [CreateAssetMenu]
    public class Tutorial : ScriptableObject
    {
        [TextArea]
        public string Message;
        public Sprite[] ClipFrames;
    }
}