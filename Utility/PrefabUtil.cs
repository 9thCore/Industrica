using System;
using System.Collections;
using UnityEngine;

namespace Industrica.Utility
{
    public static class PrefabUtil
    {
        public static IEnumerator RunOnPrefab(TechType techType, Action<GameObject> action)
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
    }
}
