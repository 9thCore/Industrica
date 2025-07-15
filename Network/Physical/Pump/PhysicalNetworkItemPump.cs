using Industrica.Container.Item;

namespace Industrica.Network.Physical.Pump
{
    public class PhysicalNetworkItemPump : PhysicalNetworkPump<Pickupable, ItemPumpSlot>
    {
        public override ItemPumpSlot GetStorage => new ItemPumpSlot(Input, Output, StorageRoot.transform);
    }
}
