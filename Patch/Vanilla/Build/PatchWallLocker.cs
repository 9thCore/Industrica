using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical;
using Industrica.Utility;
using UnityEngine;
using UWE;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchWallLocker
    {
        public static void Patch()
        {
            PrefabUtil.RunOnPrefab(TechType.SmallLocker, go =>
            {
                go.EnsureComponent<StorageContainerProvider>();

                Vector3 topAndFront = Vector3.up * 0.42f + Vector3.forward * 0.17f;
                Vector3 rightSide = Vector3.right * -0.42f;
                Vector3 leftSide = -rightSide;

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    leftSide + topAndFront,
                    Quaternion.Euler(0f, 0f, 270f),
                    Network.PortType.Input,
                    false);

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    rightSide + topAndFront,
                    Quaternion.Euler(0f, 0f, 90f),
                    Network.PortType.Output,
                    false);
            });
        }
    }
}
