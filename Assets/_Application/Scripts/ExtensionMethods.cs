using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace GMTK2020
{
    public static class VectorExtensionMethods
    {
        public static void Deconstruct(this Vector2Int vec2, out int x, out int y)
        {
            x = vec2.x;
            y = vec2.y;
        }
        public static void Deconstruct(this Vector3Int vec3, out int x, out int y, out int z)
        {
            x = vec3.x;
            y = vec3.y;
            z = vec3.z;
        }
        public static void Deconstruct(this Vector2 vec2, out float x, out float y)
        {
            x = vec2.x;
            y = vec2.y;
        }
        public static void Deconstruct(this Vector3 vec3, out float x, out float y, out float z)
        {
            x = vec3.x;
            y = vec3.y;
            z = vec3.z;
        }
    }

    public static class SystemExtensionMethods
    {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
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
        public static IEnumerable<T> FastReverse<T>(this IList<T> items)
        {
            for (int i = items.Count - 1; i >= 0; i--)
                yield return items[i];
        }
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static TValue RandomChoice<TValue>(this TValue[] values, Random rng)
        {
            int randomIndex = rng.Next(values.Length);
            return values[randomIndex];
        }
    }

    public static class UnityObjectExtensionMethods
    {
        // Check whether the game object is a prefab.
        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.rootCount == 0;
        }
        // Check whether the component is attached to a prefab.
        public static bool IsPrefab(this Component component)
        {
            return component.gameObject.scene.rootCount == 0;
        }

        // Chainable convenience methods for SetActive().
        public static GameObject Deactivate(this GameObject gameObject)
        {
            gameObject.SetActive(false);
            return gameObject;
        }
        public static GameObject Activate(this GameObject gameObject)
        {
            gameObject.SetActive(true);
            return gameObject;
        }
        public static T DeactivateObject<T>(this T component) where T : Component
        {
            component.gameObject.SetActive(false);
            return component;
        }
        public static T ActivateObject<T>(this T component) where T : Component
        {
            component.gameObject.SetActive(true);
            return component;
        }

        // Use this for game objects which might still have tweens
        // running on them.
        public static void TweenSafeDestroy(this GameObject gameObject)
        {
            DOTween.Kill(gameObject);
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    public static class UnityComponentExtensionMethods
    {
        public static void SetAlpha(this SpriteRenderer renderer, float alpha)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        }

        public static void SetAlpha(this Image image, float alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }
}