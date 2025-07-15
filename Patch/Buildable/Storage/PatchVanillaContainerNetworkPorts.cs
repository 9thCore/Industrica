using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical;
using Industrica.Utility;
using UnityEngine;
using UWE;

namespace Industrica.Patch.Buildable.Storage
{
    public static class PatchVanillaContainerNetworkPorts
    {
        public static void PatchAll()
        {
            PatchLocker();
            PatchSmallLocker();
            PatchFiltrationMachine();
        }

        private static void PatchFiltrationMachine()
        {
            CoroutineHost.StartCoroutine(PrefabUtil.RunOnPrefab(TechType.BaseFiltrationMachine, go =>
            {
                go.EnsureComponent<FiltrationMachineContainerProvider>();

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    Vector3.zero,
                    Quaternion.identity,
                    Network.PortType.Output,
                    false);
            }));
        }

        private static void PatchLocker()
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

        private static void PatchSmallLocker()
        {
            CoroutineHost.StartCoroutine(PrefabUtil.RunOnPrefab(TechType.SmallLocker, go =>
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
            }));
        }
    }
}
