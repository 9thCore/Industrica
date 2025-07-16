using Industrica.Network.BaseModule.Vanilla;
using Industrica.Network.Container.Provider.Item.Vanilla;
using Industrica.Network.Physical.Item;
using Industrica.Utility;
using UnityEngine;
using UWE;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchNuclearReactor
    {
        public static void Patch()
        {
            PrefabUtil.RunOnPrefab("864f7780-a4c3-4bf2-b9c7-f4296388b70f", go =>
            {
                go.EnsureComponent<NuclearReactorConstructionProvider>();
                go.EnsureComponent<NuclearReactorContainerProvider>();

                Vector3 top = Vector3.up * 0.96f;
                Vector3 rightSide = Vector3.forward * 0.96f;
                Vector3 leftSide = -rightSide;

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    top + rightSide,
                    Quaternion.Euler(100f, 0f, 0f),
                    Network.PortType.Input,
                    false);

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    top + leftSide,
                    Quaternion.Euler(260f, 0f, 0f),
                    Network.PortType.Output,
                    true);
            });
        }
    }
}
