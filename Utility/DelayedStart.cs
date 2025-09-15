using Nautilus.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Utility
{
    public class DelayedStart : MonoBehaviour
    {
        public void Awake()
        {
            if (LargeWorldStreamer.main.globalRoot == null)
            {
                gameObject.SetActive(false);
                DeferredEnables.Add(this);
            }
        }

        public static IEnumerator EnableObjects(WaitScreenHandler.WaitScreenTask task)
        {
            if (DeferredEnables.Count == 0)
            {
                yield break;
            }

            // This should exist, but just in case
            while (LargeWorldStreamer.main.globalRoot == null)
            {
                yield return null;
            }

            int total = DeferredEnables.Count;
            if (total < Timeout)
            {
                yield break;
            }

            int nextTimeout = Timeout;
            for (int i = 0; i < total; i++)
            {
                if (i == nextTimeout)
                {
                    nextTimeout += Timeout;
                    task.Status = "IndustricaLoading_StartDelayerEnable".Translate(i - 1, total);
                    yield return null;
                }

                DeferredEnables[i].gameObject.SetActive(true);
            }

            DeferredEnables.Clear();
        }

        private static readonly List<DelayedStart> DeferredEnables = new();
        public const int Timeout = 40;
    }
}
