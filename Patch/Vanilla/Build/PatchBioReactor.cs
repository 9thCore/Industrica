using HarmonyLib;
using Industrica.Network;
using Industrica.Network.BaseModule.Vanilla;
using Industrica.Network.Container.Provider.Item.Vanilla;
using Industrica.Network.Pipe.Item;
using Industrica.Utility;
using Nautilus.Handlers;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchBioReactor
    {
        public static IEnumerator Patch()
        {
            yield return PrefabUtil.RunOnPrefabAsync("769f9f44-30f6-46ed-aaf6-fbba358e1676", go =>
            {
                go.EnsureComponent<BioReactorConstructionProvider>();
                go.EnsureComponent<BioReactorContainerProvider>();

                TransferItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    new Vector3(1.53f, -0.61f, 0f),
                    Quaternion.Euler(0f, 0f, 315f),
                    PortType.Input);
            });
        }

        public static void Test(WaitScreenHandler.WaitScreenTask task)
        {
            task.Status = "Testing 3 ???";
        }

        public static IEnumerator TestAsync(WaitScreenHandler.WaitScreenTask task)
        {
            task.Status = "Testing 4...";
            yield break;
        }

        public static IEnumerator TestAsyncTwo(WaitScreenHandler.WaitScreenTask task)
        {
            task.Status = "Testing 5!";
            yield return new WaitForSecondsRealtime(1f);
        }

        public static void PatchMethod(Harmony harmony)
        {
            MethodInfo original = typeof(BaseBioReactorGeometry)
               .GetMethod(nameof(BaseBioReactorGeometry.Start), BindingFlags.NonPublic | BindingFlags.Instance);
            HarmonyMethod postfix = new HarmonyMethod(typeof(PatchBioReactor)
                .GetMethod(nameof(HarmonyPatch), BindingFlags.Public | BindingFlags.Static));
            harmony.Patch(original, postfix: postfix);
        }

        public static void HarmonyPatch(BaseBioReactorGeometry __instance)
        {
            BaseBioReactor module = __instance.GetModule();
            if (module == null)
            {
                Plugin.Logger.LogError($"{__instance.gameObject} does not have a module associated??? Cannot apply deconstruction patch.");
                return;
            }

            PortHandler handler = module.GetComponent<PortHandler>();
            handler.CopyTo(__instance.gameObject);
        }
    }
}
