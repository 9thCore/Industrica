using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Network.Wire
{
    public class WirePortHandler : PortHandler
    {
        public List<WirePort> ports;

        public override string DeconstructionDeniedReason => "IndustricaWirePort_CannotDeconstructConnected";

        public void Register(WirePort port)
        {
            ports ??= new();
            ports.Add(port);
        }

        public override bool CanDeconstructPorts()
        {
            return ports == null || ports.All(p => p == null || !p.Occupied);
        }

        public override PortHandler CopyTo(GameObject prefab)
        {
            ports ??= new();
            WirePortHandler handler = prefab.EnsureComponent<WirePortHandler>();
            handler.ports = ports;
            return handler;
        }
    }
}
