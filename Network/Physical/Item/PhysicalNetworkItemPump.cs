using Industrica.Network.Container;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalNetworkItemPump : PhysicalNetworkPump<Pickupable, PumpContainer<Pickupable>>
    {
        public override PumpContainer<Pickupable> GetContainer => new PumpContainer<Pickupable>(Input, Output);
    }
}
