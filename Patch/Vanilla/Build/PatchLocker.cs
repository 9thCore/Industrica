using HarmonyLib;
using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical.Item;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    [HarmonyPatch(typeof(StorageContainer), nameof(StorageContainer.Awake))]
    public static class PatchLocker
    {
        public static void Postfix(StorageContainer __instance)
        {
            TechTag tag = __instance.GetComponent<TechTag>();
            if (tag == null
                || tag.type != TechType.Locker)
            {
                return;
            }

            GameObject go = __instance.gameObject;

            go.EnsureComponent<StorageContainerProvider>();

            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
            Vector3 front = Vector3.forward * 0.25f;
            Vector3 topSection = Vector3.up * 1.48f;
            Vector3 bottomSection = Vector3.up * 0.39f;
            Vector3 rightSide = Vector3.right * -0.17f;
            Vector3 leftSide = -rightSide;

            PhysicalNetworkItemPort.CreatePort(
                prefab: go,
                root: go,
                rightSide + topSection + front,
                rotation,
                Network.PortType.Input);

            PhysicalNetworkItemPort.CreatePort(
                prefab: go,
                root: go,
                leftSide + topSection + front,
                rotation,
                Network.PortType.Input);

            PhysicalNetworkItemPort.CreatePort(
                prefab: go,
                root: go,
                rightSide + bottomSection + front,
                rotation,
                Network.PortType.Output);

            PhysicalNetworkItemPort.CreatePort(
                prefab: go,
                root: go,
                leftSide + bottomSection + front,
                rotation,
                Network.PortType.Output);
        }
    }
}
