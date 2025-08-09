using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalNetworkItemPortHandler : PhysicalNetworkPortHandler<Pickupable>
    {
        public List<PhysicalNetworkItemPort> ports;

        public override string DeconstructionDeniedReason => "IndustricaItemPort_CannotDeconstructConnected";

        public override bool CanDeconstructPorts()
        {
            return CanDeconstruct(ports);
        }

        public override PortHandler CopyTo(GameObject prefab)
        {
            ports ??= new();
            PhysicalNetworkItemPortHandler handler = prefab.EnsureComponent<PhysicalNetworkItemPortHandler>();
            handler.ports = ports;
            return handler;
        }

        public override void Register(PhysicalNetworkPort<Pickupable> port)
        {
            ports ??= new();
            ports.Add(port as PhysicalNetworkItemPort);
        }
    }
}
