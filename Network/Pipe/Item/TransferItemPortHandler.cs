using System.Collections.Generic;

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

        public override void Register(TransferPort<Pickupable> port)
        {
            ports ??= new();
            ports.Add(port as TransferItemPort);
        }
    }
}
