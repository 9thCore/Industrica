using Industrica.Network.BaseModule.Vanilla;
using Industrica.Network.Container.Provider.Item.Vanilla;
using Industrica.Network.Physical.Item;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Patch.Vanilla.Build
{
    public static class PatchBioReactor
    {
        public static void Patch()
        {
            PrefabUtil.RunOnPrefab("769f9f44-30f6-46ed-aaf6-fbba358e1676", go =>
            {
                go.EnsureComponent<BioReactorConstructionProvider>();
                go.EnsureComponent<BioReactorContainerProvider>();

                PhysicalNetworkItemPort.CreatePort(
                    go,
                    new Vector3(1.53f, -0.61f, 0f),
                    Quaternion.Euler(0f, 0f, 315f),
                    Network.PortType.Input,
                    false);
            });
        }
    }
}
