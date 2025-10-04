using Industrica.Fluid;
using Industrica.Network.BaseModule;
using UnityEngine;

namespace Industrica.Network.Pipe.Fluid
{
    internal class PhysicalFluidPortRepresentation : PhysicalPortRepresentation<FluidStack, TransferFluidPort>
    {
        public static PhysicalFluidPortRepresentation Create(GameObject prefab, TransferFluidPort parent, BaseModuleProvider provider, GameObject portRoot)
        {
            return Create<PhysicalFluidPortRepresentation>(prefab, parent, provider, portRoot);
        }
    }
}
