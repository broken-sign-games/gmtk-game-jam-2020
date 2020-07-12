using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GMTK2020
{
    public static class ExtensionMethods
    {
        public static void Deconstruct(this Vector2Int vec2, out int x, out int y)
        {
            x = vec2.x;
            y = vec2.y;
        }

        // Fisher-Yates shuffle.
        public static List<T> Shuffle<T>(this IList<T> list, Random rng)
        {
            List<T> shuffled = new List<T>(list);

            int n = shuffled.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (shuffled[k], shuffled[n]) = (shuffled[n], shuffled[k]);
            }

            return shuffled;
        }
    }
}