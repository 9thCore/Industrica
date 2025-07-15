using Industrica.Network.Physical;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Container.Item
{
    public class ItemPumpSlot : PumpSlot<Pickupable>
    {
        protected readonly Transform root;
        private InventoryItem stored;

        public ItemPumpSlot(PhysicalNetworkPort<Pickupable> input, PhysicalNetworkPort<Pickupable> output, Transform root) : base(input, output)
        {
            this.root = root;
        }
    }
}
