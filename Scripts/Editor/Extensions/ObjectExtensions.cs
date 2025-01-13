using UnityEngine;

namespace Editor.Extensions
{
    public static class ObjectExtensions
    {
        public static Transform[] GetChildren(this Object target)
        {
            Transform parent = ((GameObject)target).transform;

            Transform[] result = new Transform[parent.childCount];

            for (int i = 0; i < parent.childCount; i++)
            {
                result[i] = parent.GetChild(i);
            }

            return result;
        }

        public static Vector2 GetBounds(this GameObject target)
        {
            var result = new Vector2();

            GameObject gameObject = (GameObject)target;

            var spriteRenderer = GetComponentRecursive<SpriteRenderer>(gameObject);

            result.x = spriteRenderer.bounds.size.x;

            result.y = spriteRenderer.bounds.size.y;

            return result;
        }

        private static T GetComponentRecursive<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T component))
                return component;

            return gameObject.GetComponentsInChildren<T>()[0];
        }
    }
}