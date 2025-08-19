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
            container.onAddItem += OnContainerUpdate;
            container.onRemoveItem += OnContainerUpdate;
        }

        private void OnContainerUpdate(InventoryItem item)
        {
            OnUpdate?.Invoke(this);
        }

        protected override void Add(Pickupable value)
        {
            container.AddItem(new InventoryItem(value));
        }

        public override int Count(NetworkFilter<Pickupable> filter)
        {
            int count = 0;

            foreach (InventoryItem item in container)
            {
                if (filter.Matches(item.item))
                {
                    count++;
                }
            }

            return count;
        }

        public override int CountRemovable(NetworkFilter<Pickupable> filter)
        {
            int count = 0;

            foreach (InventoryItem item in container)
            {
                if (container.AllowedToRemove(item.item, false) && filter.Matches(item.item))
                {
                    count++;
                }
            }

            return count;
        }

        public override IEnumerator<Pickupable> GetEnumerator()
        {
            foreach (InventoryItem item in container)
            {
                yield return item.item;
            }
        }

        protected override void Remove(Pickupable value)
        {
            if (value.inventoryItem == null)
            {
                return;
            }

            container.RemoveItem(value.inventoryItem, true, false);
        }

        public override bool TryExtract(NetworkFilter<Pickupable> filter, out Pickupable value, bool simulate = false)
        {
            foreach (InventoryItem item in container)
            {
                if (container.AllowedToRemove(item.item, false) && filter.Matches(item.item))
                {
                    value = item.item;
                    if (!simulate)
                    {
                        Remove(value);
                    }
                    return true;
                }
            }

            value = default;
            return false;
        }

        public override bool TryInsert(Pickupable value, bool simulate = false)
        {
            if (!container.AllowedToAdd(value, false)
                || !container.HasRoomFor(value, null))
            {
                return false;
            }

            if (!simulate)
            {
                Add(value);
            }

            return true;
        }
    }
}
