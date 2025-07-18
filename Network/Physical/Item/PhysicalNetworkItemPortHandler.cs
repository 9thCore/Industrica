using System.Linq;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalNetworkItemPortHandler : PhysicalNetworkPortHandler<Pickupable>
    {
        public PhysicalNetworkItemPort[] ports;

        public override bool CanDeconstructPorts()
        {
            return ports == null || ports.All(c => c == null || !c.HasNetwork);
        }

        public override void Fetch()
        {
            ports = GetComponentsInChildren<PhysicalNetworkItemPort>(true);
        }
    }
}
