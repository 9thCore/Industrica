using Industrica.Fluid;

namespace Industrica.Network.Container
{
    public abstract class FluidContainer : Container<FluidStack>
    {
        public abstract int GetAvailableFluidSpace();
    }
}
