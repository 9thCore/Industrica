using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Utility
{
    public static class GameObjectUtil
    {
        public static GameObject CreateChild(GameObject parent, string name, PrimitiveType? primitive = null, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
        {
            GameObject child;
            if (primitive.HasValue)
            {
                child = GameObject.CreatePrimitive(primitive.Value);
                child.name = name;
            } else
            {
                child = new GameObject(name);
            }

            child.transform.SetParent(parent.transform);
            child.transform.localPosition = position ?? Vector3.zero;
            child.transform.localRotation = rotation ?? Quaternion.identity;
            child.transform.localScale = scale ?? Vector3.one;
            return child;
        }

        public static void Resize(Transform transform, float? x = null, float? y = null, float? z = null)
        {
            Vector3 scale = transform.localScale;
            if (x.HasValue)
            {
                scale.x = x.Value;
            }
            if (y.HasValue)
            {
                scale.y = y.Value;
            }
            if (z.HasValue)
            {
                scale.z = z.Value;
            }

            transform.localScale = scale;
        }

        public static bool TryGetComponentInParent<T>(this GameObject go, out T component)
        {
            component = go.GetComponentInParent<T>();
            return component != null;
        }

        public static void DestroyImmediateChildrenWith<T>(this GameObject root, bool includeInactive = false) where T : Component
        {
            ActionOnChildrenWith<T>(root, GameObject.DestroyImmediate, includeInactive);
        }

        public static void ActionOnChildrenWith<T>(this GameObject root, Action<GameObject> action, bool includeInactive = false) where T : Component
        {
            HashSet<GameObject> toAction = new();
            root.GetComponentsInChildren<T>(includeInactive).ForEach(c => toAction.Add(c.gameObject));
            toAction.ForEach(action);
        }

        public static bool TryFindObjectWith<T>(Func<T, bool> filter, out T result) where T : MonoBehaviour
        {
            result = null;

            T[] allObjects = Resources.FindObjectsOfTypeAll<T>();
            if (allObjects.Count() == 0)
            {
                return false;
            }

            bool foundMore = false;

            foreach (T obj in allObjects)
            {
                if (filter.Invoke(obj))
                {
                    if (result == null)
                    {
                        result = obj;
                    } else
                    {
                        foundMore = true;
                    }
                }
            }

            if (foundMore)
            {
                Plugin.Logger.LogWarning($"Filter wasn't specific enough, more than one object was found. Returning with the first object found");
            }

            return true;
        }

        public static void SetupConstructableBounds(this GameObject prefab)
        {
            ConstructableBounds bounds = prefab.EnsureComponent<ConstructableBounds>();
            Vector3 position = prefab.transform.position;
            OrientedBounds oriented = new()
            {
                position = position,
                rotation = prefab.transform.rotation
            };
            Bounds encapsulator = new(position, Vector3.zero);
            prefab.GetComponentsInChildren<Renderer>().ForEach(r => encapsulator.Encapsulate(r.bounds));
            oriented.extents = encapsulator.extents;
            oriented.size = encapsulator.size;
            bounds.bounds = oriented;
        }
    }
}
