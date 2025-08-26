using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Network.Pipe.Item
{
    public class TransferItemPortHandler : TransferPortHandler<Pickupable>
    {
        public List<TransferItemPort> ports;

        public override string DeconstructionDeniedReason => "IndustricaItemPort_CannotDeconstructConnected";

        public override bool CanDeconstructPorts()
        {
            return CanDeconstruct(ports);
        }

        public override PortHandler CopyTo(GameObject prefab)
        {
            ports ??= new();
            TransferItemPortHandler handler = prefab.EnsureComponent<TransferItemPortHandler>();
            handler.ports = ports;
            return handler;
        }

        public override void Register(TransferPort<Pickupable> port)
        {
            ports ??= new();
            ports.Add(port as TransferItemPort);
        }
    }
}
