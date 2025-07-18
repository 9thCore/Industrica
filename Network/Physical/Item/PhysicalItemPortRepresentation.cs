using UnityEngine;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalItemPortRepresentation : PhysicalPortRepresentation<Pickupable>
    {
        public static PhysicalItemPortRepresentation Create(GameObject prefab)
        {
            return Create<PhysicalItemPortRepresentation>(prefab);
        }
    }
}
