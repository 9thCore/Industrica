using Industrica.Network.Physical.Item;

namespace Industrica.Network.Container.Provider.Item
{
    public class ItemPumpContainerProvider : ContainerProvider<Pickupable>
    {
        private Container<Pickupable> container;
        public override Container<Pickupable> Container => container ??= new PumpContainer<Pickupable>(GetComponent<PhysicalNetworkItemPump>().Storage);
    }
}
