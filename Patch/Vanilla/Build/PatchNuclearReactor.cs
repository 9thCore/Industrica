using HarmonyLib;
using Industrica.Network;
using Industrica.Network.BaseModule.Vanilla;
using Industrica.Network.Container.Provider.Item.Vanilla;
using Industrica.Network.Physical.Item;
using Industrica.Utility;
using System.Reflection;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchNuclearReactor
    {
        public static void Patch(Harmony harmony)
        {
            MethodInfo original = typeof(BaseNuclearReactorGeometry)
                .GetMethod(nameof(BaseNuclearReactorGeometry.Start), BindingFlags.NonPublic | BindingFlags.Instance);
            HarmonyMethod postfix = new HarmonyMethod(typeof(PatchNuclearReactor)
                .GetMethod(nameof(HarmonyPatch), BindingFlags.Public | BindingFlags.Static));
            harmony.Patch(original, postfix: postfix);

            PrefabUtil.RunOnPrefab("864f7780-a4c3-4bf2-b9c7-f4296388b70f", go =>
            {
                go.EnsureComponent<NuclearReactorConstructionProvider>();
                go.EnsureComponent<NuclearReactorContainerProvider>();

                Vector3 top = Vector3.up * 0.96f;
                Vector3 rightSide = Vector3.forward * 0.96f;
                Vector3 leftSide = -rightSide;

                PhysicalNetworkItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    top + rightSide,
                    Quaternion.Euler(100f, 0f, 0f),
                    Network.PortType.Input,
                    false);

                PhysicalNetworkItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    top + leftSide,
                    Quaternion.Euler(260f, 0f, 0f),
                    Network.PortType.Output,
                    true);
            });
        }

        public static void HarmonyPatch(BaseNuclearReactorGeometry __instance)
        {
            BaseNuclearReactor module = __instance.GetModule();
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
