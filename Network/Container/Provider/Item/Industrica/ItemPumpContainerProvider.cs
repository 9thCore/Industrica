using Industrica.Network.Pipe.Item;

namespace Industrica.Network.Container.Provider.Item.Industrica
{
    public class ItemPumpContainerProvider : ContainerProvider<Pickupable>
    {
        private Container<Pickupable> container;
        public override Container<Pickupable> Container => container ??= GetComponent<TransferItemPump>().Container;
    }
}
