using System;
using System.Linq;
using UnityEngine;

namespace Industrica.Utility
{
    public static class GameObjectUtil
    {
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
    }
}
