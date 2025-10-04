using Industrica.Fluid;
using System.Collections.Generic;

namespace Industrica.Network.Pipe.Fluid
{
    internal class TransferFluidPortHandler : TransferPortHandler<FluidStack>
    {
        public List<TransferFluidPort> ports;

        public override string DeconstructionDeniedReason => "IndustricaItemPort_CannotDeconstructConnected";

        public override bool CanDeconstructPorts()
        {
            return CanDeconstruct(ports);
        }

        public override void Register(TransferPort<FluidStack> port)
        {
            ports ??= new();
            ports.Add(port as TransferFluidPort);
        }
    }
}
