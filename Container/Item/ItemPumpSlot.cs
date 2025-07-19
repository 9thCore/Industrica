using Industrica.Network.Physical;
using UnityEngine;

namespace Industrica.Container.Item
{
    public class ItemPumpSlot : PumpSlot<Pickupable>
    {
        protected readonly Transform root;

        public ItemPumpSlot(PhysicalNetworkPort<Pickupable> input, PhysicalNetworkPort<Pickupable> output, Transform root) : base(input, output)
        {
            this.root = root;
        }
    }
}
