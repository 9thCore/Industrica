using UnityEngine;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalItemPortRepresentation : PhysicalPortRepresentation<Pickupable, PhysicalNetworkItemPort>
    {
        public static PhysicalItemPortRepresentation Create(GameObject prefab, PhysicalNetworkItemPort parent, GameObject portRoot)
        {
            return Create<PhysicalItemPortRepresentation>(prefab, parent, portRoot);
        }
    }
}
