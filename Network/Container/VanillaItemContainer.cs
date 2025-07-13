using Industrica.Network.Filter;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public class VanillaItemContainer : Container<Pickupable>
    {
        private readonly IItemsContainer container;
        public VanillaItemContainer(IItemsContainer container)
        {
            this.container = container;
        }

        public override IEnumerator<Pickupable> GetEnumerator()
        {
            foreach (InventoryItem item in container)
            {
                yield return item.item;
            }
        }

        public override bool TryExtract(NetworkFilter<Pickupable> filter, out Pickupable value)
        {
            foreach (Pickupable item in this)
            {
                if (container.AllowedToRemove(item, false) && filter.Matches(item))
                {
                    value = item;
                    return container.RemoveItem(item.inventoryItem, true, false);
                }
            }

            value = default;
            return false;
        }

        public override bool CanInsert(Pickupable value)
        {
            return container.AllowedToAdd(value, false) && container.HasRoomFor(value, null);
        }

        public override bool TryInsert(Pickupable value)
        {
            if (!CanInsert(value))
            {
                return false;
            }

            return container.AddItem(new InventoryItem(value));
        }

        public override bool Contains(NetworkFilter<Pickupable> filter, out bool canExtract)
        {
            foreach (Pickupable item in this)
            {
                if (filter.Matches(item))
                {
                    canExtract = container.AllowedToRemove(item, false);
                    return true;
                }
            }

            canExtract = default;
            return false;
        }
    }
}
