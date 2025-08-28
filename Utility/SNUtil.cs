using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Utility
{
    public static class SNUtil
    {
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
    }
}
