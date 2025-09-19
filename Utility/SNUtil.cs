using Industrica.Recipe.Handler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace Industrica.Utility
{
    public static class SNUtil
    {
        public static IEnumerator TryGetItemPrefab(TechType techType, TaskResult<GameObject> result)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType, false);
            yield return task;

            GameObject prefab = task.GetResult();
            if (prefab == null
                || !prefab.TryGetComponent(out Pickupable _))
            {
                yield break;
            }

            result.Set(prefab);
        }

        public static bool HasRoomFor(this ItemsContainer container, IEnumerable<Pickupable> items)
        {
            List<Vector2int> sizes = items.Select(item => TechData.GetItemSize(item.GetTechType())).ToList();
            return container.HasRoomFor(sizes);
        }

        public static bool HasRoomFor(this ItemsContainer container, IEnumerable<RecipeHandler.RecipeItemOutput> outputs)
        {
            List<Vector2int> result = new();
            foreach (var output in outputs)
            {
                output.GetSizes(result);
            }

            return container.HasRoomFor(result);
        }

        public static bool HasRoomFor(this ItemsContainer container, IEnumerable<InventoryItem> items)
        {
            return container.HasRoomFor(items.Select(item => item.item));
        }

        public static bool DisallowAction(Pickupable pickupable, bool verbose)
        {
            return false;
        }

        public static IEnumerator GetBasePieceDefinition(Base.Piece piece, IOut<Base.PieceDef> result)
        {
            if (!Base.initialized)
            {
                Plugin.Logger.LogWarning("Somehow, base pieces have not been initialized yet. Waiting for that...");
                yield return new WaitUntil(() => Base.initialized);
            }

            result.Set(Base.pieces[(int)piece]);
        }

        public static void GetNearestValidPowerRelay(Vector3 position, float range, Action<PowerRelay> callback, Func<PowerRelay, bool> validator = null, int timeout = 10)
        {
            CoroutineHost.StartCoroutine(GetNearestValidPowerRelayAsync(position, range, callback, validator, timeout));
        }

        private static IEnumerator GetNearestValidPowerRelayAsync(Vector3 position, float range, Action<PowerRelay> callback, Func<PowerRelay, bool> validator, int timeout)
        {
            float rangeSquared = range * range;

            PowerRelay result = null;
            float currentDistance = float.MaxValue;

            int runningCount = 0;

            foreach (PowerRelay powerRelay in PowerRelay.relayList)
            {
                if (VerifyPowerRelay(powerRelay, validator))
                {
                    Vector3 otherPosition = powerRelay.GetConnectPoint(position);
                    float distance = Vector3.SqrMagnitude(otherPosition - position);
                    if (distance <= rangeSquared
                        && distance < currentDistance)
                    {
                        result = powerRelay;
                        currentDistance = distance;
                    }
                }

                runningCount++;
                if (runningCount >= timeout)
                {
                    yield return null;
                }
            }

            if (result != null)
            {
                callback(result);
            }
        }

        private static bool VerifyPowerRelay(PowerRelay powerRelay, Func<PowerRelay, bool> validator)
        {
            if (!powerRelay.enabled
                    || powerRelay.dontConnectToRelays
                    || Builder.ghostModel == powerRelay.gameObject
                    || !powerRelay.gameObject.activeInHierarchy)
            {
                return false;
            }

            if (validator == null)
            {
                return true;
            }

            return validator(powerRelay);
        }

        private record ItemContainerMapping(InventoryItem InventoryItem, IItemsContainer Container);
    }
}
