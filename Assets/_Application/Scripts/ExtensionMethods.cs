using UnityEngine;

namespace GMTK2020
{
    public static class ExtensionMethods
    {
        public static void Deconstruct(this Vector2Int vec2, out int x, out int y)
        {
            x = vec2.x;
            y = vec2.y;
        }
    }
}