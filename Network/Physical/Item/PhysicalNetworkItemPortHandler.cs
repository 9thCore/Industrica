using System.Collections.Generic;
using System.Linq;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalNetworkItemPortHandler : PhysicalNetworkPortHandler<Pickupable>
    {
        public List<PhysicalNetworkItemPort> ports = new();

        public override bool CanDeconstructPorts()
        {
            return CanDeconstruct(ports);
        }

        public override void Register(PhysicalNetworkPort<Pickupable> port)
        {
            ports.Add(port as PhysicalNetworkItemPort);
        }
    }
}
