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
            RunOnPrefab(CoroutineHost.Initialize(), techType, action);
        }

        public static void RunOnPrefab(string classID, Action<GameObject> action)
        {
            RunOnPrefab(CoroutineHost.Initialize(), classID, action);
        }

        public static void RunOnPrefab(MonoBehaviour component, TechType techType, Action<GameObject> action)
        {
            component.StartCoroutine(RunOnPrefabAsync(techType, action));
        }

        public static void RunOnPrefab(MonoBehaviour component, string classID, Action<GameObject> action)
        {
            component.StartCoroutine(RunOnPrefabAsync(classID, action));
        }

        public static IEnumerator RunOnPrefabAsync(TechType techType, Action<GameObject> action)
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

        public static IEnumerator RunOnPrefabAsync(string classID, Action<GameObject> action)
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
