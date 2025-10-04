using Industrica.Fluid;
using Nautilus.Extensions;

namespace Industrica.Network.Container.Provider.Fluid.Industrica
{
    public class FluidTankContainerProvider : ContainerProvider<FluidStack>
    {
        private FluidTank tank;
        public FluidTank Tank => tank.Exists() ?? (tank = GetComponentInParent<FluidTank>());
        public override Container<FluidStack> Container => new FluidTankContainer(Tank);
    }
}
