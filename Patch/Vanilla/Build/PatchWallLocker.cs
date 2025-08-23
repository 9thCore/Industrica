using HarmonyLib;
using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical.Item;
using Industrica.Network.Wire;
using Industrica.Network.Wire.Output;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    [HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.Awake))]
    public static class PatchWallLocker
    {
        public static void Postfix(StorageContainer __instance)
        {
            TechTag tag = __instance.GetComponent<TechTag>();
            if (tag == null
                || tag.type != TechType.SmallLocker)
            {
                return;
            }

            GameObject go = __instance.gameObject;

            go.EnsureComponent<StorageContainerProvider>();

            Vector3 topAndFront = Vector3.up * 0.42f + Vector3.forward * 0.17f;
            Vector3 rightSide = Vector3.right * -0.42f;
            Vector3 leftSide = -rightSide;

            PhysicalNetworkItemPort.CreatePort(
                prefab: go,
                root: go,
                leftSide + topAndFront,
                Quaternion.Euler(0f, 0f, 270f),
                Network.PortType.Input);

            PhysicalNetworkItemPort.CreatePort(
                prefab: go,
                root: go,
                rightSide + topAndFront,
                Quaternion.Euler(0f, 0f, 90f),
                Network.PortType.Output);

            WirePort output = WirePort.CreatePort(
                go,
                Vector3.forward * 0.35f - Vector3.up * 0.17f,
                Quaternion.Euler(90f, 0f, 0f),
                Network.PortType.Output);

            go.EnsureComponent<ItemContainerWireOutput>().SetPort(output);
        }
    }
}
