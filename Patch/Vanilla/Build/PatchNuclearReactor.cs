using HarmonyLib;
using Industrica.Network;
using Industrica.Network.BaseModule;
using Industrica.Network.BaseModule.Vanilla;
using Industrica.Network.Container.Provider.Item.Vanilla;
using Industrica.Network.Pipe.Item;
using Industrica.Utility;
using System.Collections;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    [HarmonyPatch(typeof(BaseNuclearReactorGeometry), nameof(BaseNuclearReactorGeometry.Start))]
    public static class PatchNuclearReactor
    {
        public static IEnumerator Patch()
        {
            yield return PrefabUtil.RunOnPrefabAsync("864f7780-a4c3-4bf2-b9c7-f4296388b70f", go =>
            {
                go.EnsureComponent<NuclearReactorContainerProvider>();

                BaseModuleProvider provider = go.EnsureComponent<NuclearReactorModuleProvider>()
                .WithModule(go.GetComponent<BaseNuclearReactor>());

                Vector3 top = Vector3.up * 0.96f;
                Vector3 rightSide = Vector3.forward * 0.96f;
                Vector3 leftSide = -rightSide;

                TransferItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    top + rightSide,
                    Quaternion.Euler(100f, 0f, 0f),
                    PortType.Input);

                TransferItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    top + leftSide,
                    Quaternion.Euler(260f, 0f, 0f),
                    PortType.Output);

                provider.SetPortHandler(go.GetComponent<PortHandler>());
            });
        }

        public static void Postfix(BaseNuclearReactorGeometry __instance)
        {
            BaseNuclearReactor module = __instance.GetModule();
            if (module == null)
            {
                Plugin.Logger.LogError($"Could not find module of {__instance} - cannot apply port deconstruction patch.");
                return;
            }

            BaseModuleProvider moduleProvider = module.GetComponent<BaseModuleProvider>();
            if (moduleProvider == null)
            {
                Plugin.Logger.LogError($"Could not find module provider of {module} - cannot apply port deconstruction patch.");
                return;
            }

            __instance.gameObject.AddComponent<GeometryHandler>().moduleProvider = moduleProvider;
        }
    }
}
