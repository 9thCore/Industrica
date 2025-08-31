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
    [HarmonyPatch(typeof(BaseBioReactorGeometry), nameof(BaseBioReactorGeometry.Start))]
    public static class PatchBioReactor
    {
        public static IEnumerator Patch()
        {
            yield return PrefabUtil.RunOnPrefabAsync("769f9f44-30f6-46ed-aaf6-fbba358e1676", go =>
            {
                go.EnsureComponent<BioReactorContainerProvider>();

                BaseModuleProvider provider = go.EnsureComponent<BioReactorModuleProvider>()
                .WithModule(go.GetComponent<BaseBioReactor>());

                TransferItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    new Vector3(1.53f, -0.61f, 0f),
                    Quaternion.Euler(0f, 0f, 315f),
                    PortType.Input);

                provider.SetPortHandler(go.GetComponent<PortHandler>());
            });
        }

        public static void Postfix(BaseBioReactorGeometry __instance)
        {
            BaseBioReactor module = __instance.GetModule();
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
