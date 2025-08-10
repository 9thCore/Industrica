using Industrica.Network.Physical.Item;

namespace Industrica.Network.Container.Provider.Item.Industrica
{
    public class ItemPumpContainerProvider : ContainerProvider<Pickupable>
    {
        private Container<Pickupable> container;
        public override Container<Pickupable> Container => container ??= GetComponent<PhysicalNetworkItemPump>().Container;
    }
}
