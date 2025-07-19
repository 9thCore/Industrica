using Industrica.Network.BaseModule;
using UnityEngine;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalItemPortRepresentation : PhysicalPortRepresentation<Pickupable, PhysicalNetworkItemPort>
    {
        public static PhysicalItemPortRepresentation Create(GameObject prefab, PhysicalNetworkItemPort parent, BaseModuleProvider provider, GameObject portRoot)
        {
            return Create<PhysicalItemPortRepresentation>(prefab, parent, provider, portRoot);
        }
    }
}
