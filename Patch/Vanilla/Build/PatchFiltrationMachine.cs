using Industrica.Network.BaseModule;
using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical.Item;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchFiltrationMachine
    {
        public static void Patch()
        {
            PrefabUtil.RunOnPrefab("2f2d8419-c55b-49ac-9698-ecb431fffed2", go =>
            {
                go.EnsureComponent<FiltrationMachineConstructionProvider>();
                go.EnsureComponent<FiltrationMachineContainerProvider>();

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    new Vector3(-1.76f, 0.22f, 0f),
                    Quaternion.Euler(0f, 0f, 63f),
                    Network.PortType.Output,
                    false);
            });
        }
    }
}
