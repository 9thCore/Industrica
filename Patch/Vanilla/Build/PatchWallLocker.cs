using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical.Item;
using Industrica.Utility;
using System.Collections;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchWallLocker
    {
        public static IEnumerator Patch()
        {
            yield return PrefabUtil.RunOnPrefabAsync(TechType.SmallLocker, go =>
            {
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
            });
        }
    }
}
