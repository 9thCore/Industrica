using Industrica.Network.BaseModule;
using UnityEngine;

namespace Industrica.Network.Pipe.Item
{
    public class PhysicalItemPortRepresentation : PhysicalPortRepresentation<Pickupable, TransferItemPort>
    {
        public static PhysicalItemPortRepresentation Create(GameObject prefab, TransferItemPort parent, BaseModuleProvider provider, GameObject portRoot)
        {
            return Create<PhysicalItemPortRepresentation>(prefab, parent, provider, portRoot);
        }
    }
}
