using Industrica.Item.Network;
using Industrica.Network.Filter;
using Industrica.Network.Provider;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public class PhysicalNetworkItemPort : PhysicalNetworkPort<Pickupable>, IPhysicalNetworkPort
    {
        private IItemsContainer itemContainer;

        public override TransferPipe.PipeType AllowedPipeType => TransferPipe.PipeType.Item;

        public static PhysicalNetworkItemPort CreatePort(GameObject root, Vector3 position, Quaternion rotation, PortType type)
        {
            return CreatePort<PhysicalNetworkItemPort>(root, position, rotation, type);
        }

        public override void Start()
        {
            base.Start();
            itemContainer = gameObject.GetComponentInParent<IContainerProvider<IItemsContainer>>().Container;
        }

        public override bool HasRoomFor(Pickupable value)
        {
            if (!CanInsert)
            {
                return false;
            }

            return itemContainer.HasRoomFor(value, null);
        }

        public override bool TryExtract(NetworkFilter<Pickupable> filter, out Pickupable value)
        {
            if (!CanExtract)
            {
                value = default;
                return false;
            }

            foreach (InventoryItem item in itemContainer)
            {
                if (filter.Matches(item.item))
                {
                    value = item.item;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public override bool TryInsert(Pickupable value)
        {
            if (!HasRoomFor(value))
            {
                return false;
            }

            return itemContainer.AddItem(new InventoryItem(value));
        }
    }
}
