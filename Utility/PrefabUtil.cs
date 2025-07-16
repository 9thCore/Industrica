using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Utility
{
    public static class PrefabUtil
    {
        public static void RunOnPrefab(TechType techType, Action<GameObject> action)
        {
            CoroutineHost.StartCoroutine(RunOnPrefabAsync(techType, action));
        }

        public static void RunOnPrefab(string classID, Action<GameObject> action)
        {
            CoroutineHost.StartCoroutine(RunOnPrefabAsync(classID, action));
        }

        private static IEnumerator RunOnPrefabAsync(TechType techType, Action<GameObject> action)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
            yield return task;

            GameObject result = task.GetResult();
            if (result == null)
            {
                Plugin.Logger.LogError($"Could not find prefab of {nameof(TechType)} {techType}, could not run action.");
                yield break;
            }

            action.Invoke(result);
        }

        private static IEnumerator RunOnPrefabAsync(string classID, Action<GameObject> action)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(classID);
            yield return request;

            if (!request.TryGetPrefab(out GameObject result))
            {
                Plugin.Logger.LogError($"Could not find prefab of {nameof(classID)} {classID}, could not run action.");
                yield break;
            }

            action.Invoke(result);
        }
    }
}
