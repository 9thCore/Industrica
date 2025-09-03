using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Utility
{
    public class StartDelayer : MonoBehaviour
    {
        public void Awake()
        {
            if (LargeWorldStreamer.main.globalRoot == null)
            {
                Plugin.Logger.LogInfo($"{gameObject} got initialised too quickly, and {nameof(LargeWorldStreamer)}.{nameof(LargeWorldStreamer.globalRoot)} does not yet exist. Waiting before re-enabling...");

                gameObject.SetActive(false);
                CoroutineHost.StartCoroutine(EnableLater());
            }
        }

        private IEnumerator EnableLater()
        {
            while (LargeWorldStreamer.main.globalRoot == null)
            {
                yield return null;
            }

            gameObject.SetActive(true);
            Plugin.Logger.LogInfo($"{nameof(LargeWorldStreamer)}.{nameof(LargeWorldStreamer.globalRoot)} now exists, so {gameObject} has been re-enabled.");
        }
    }
}
