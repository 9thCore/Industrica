using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical;
using Industrica.Utility;
using UnityEngine;
using UWE;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchLocker
    {
        public static void Patch()
        {
            CoroutineHost.StartCoroutine(PrefabUtil.RunOnPrefab(TechType.Locker, go =>
            {
                go.EnsureComponent<StorageContainerProvider>();

                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                Vector3 front = Vector3.forward * 0.25f;
                Vector3 topSection = Vector3.up * 1.48f;
                Vector3 bottomSection = Vector3.up * 0.39f;
                Vector3 rightSide = Vector3.right * -0.17f;
                Vector3 leftSide = -rightSide;

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    rightSide + topSection + front,
                    rotation,
                    Network.PortType.Input,
                    false);

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    leftSide + topSection + front,
                    rotation,
                    Network.PortType.Input,
                    false);

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    rightSide + bottomSection + front,
                    rotation,
                    Network.PortType.Output,
                    false);

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    leftSide + bottomSection + front,
                    rotation,
                    Network.PortType.Output,
                    false);
            }));
        }
    }
}
