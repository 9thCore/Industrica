using Industrica.Fluid;
using Industrica.Network.Pipe.Fluid;

namespace Industrica.Network.Container.Provider.Fluid.Industrica
{
    internal class FluidPumpContainerProvider : ContainerProvider<FluidStack>
    {
        private Container<FluidStack> container;
        public override Container<FluidStack> Container => container ??= GetComponent<TransferFluidPump>().Container;
    }
}
