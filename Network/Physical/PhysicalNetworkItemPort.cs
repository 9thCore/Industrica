using Industrica.Item.Network;
using Industrica.Network.Container;
using Industrica.Network.Container.Provider;
using Industrica.Network.Filter;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public class PhysicalNetworkItemPort : PhysicalNetworkPort<Pickupable>, IPhysicalNetworkPort
    {
        private Container<Pickupable> itemContainer;

        public override TransferPipe.PipeType AllowedPipeType => TransferPipe.PipeType.Item;

        public static PhysicalNetworkItemPort CreatePort(GameObject root, Vector3 position, Quaternion rotation, PortType type)
        {
            return CreatePort<PhysicalNetworkItemPort>(root, position, rotation, type);
        }

        public override void Start()
        {
            base.Start();
            itemContainer = gameObject.GetComponentInParent<ContainerProvider<Pickupable>>().Container;
        }

        public override bool TryExtract(NetworkFilter<Pickupable> filter, out Pickupable value)
        {
            if (!CanExtract)
            {
                value = default;
                return false;
            }

            return itemContainer.TryExtract(filter, out value);
        }

        public override bool TryInsert(Pickupable value)
        {
            if (!CanInsert)
            {
                value = default;
                return false;
            }

            return itemContainer.TryInsert(value);
        }
    }
}
