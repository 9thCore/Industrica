using Industrica.Network.Physical.Pump;

namespace Industrica.Network.Container.Provider
{
    public class ItemPumpContainerProvider : ContainerProvider<Pickupable>
    {
        private Container<Pickupable> container;
        public override Container<Pickupable> Container => container ??= new PumpContainer<Pickupable>(GetComponent<PhysicalNetworkItemPump>().Storage);
    }
}
