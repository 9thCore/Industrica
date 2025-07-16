using Industrica.Container.Item;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalNetworkItemPump : PhysicalNetworkPump<Pickupable, ItemPumpSlot>
    {
        public override ItemPumpSlot GetStorage => new ItemPumpSlot(Input, Output, StorageRoot.transform);
    }
}
