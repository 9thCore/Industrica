using Industrica.Fluid;
using Industrica.Network.BaseModule;
using Industrica.Network.Container;
using Industrica.Network.Container.Provider;
using Industrica.Network.Filter;
using UnityEngine;

namespace Industrica.Network.Pipe.Fluid
{
    internal class TransferFluidPort : TransferPort<FluidStack>
    {
        public TransferFluidPortHandler handler;
        public PhysicalFluidPortRepresentation representation;

        private Container<FluidStack> fluidContainer;
        public override Container<FluidStack> Container => fluidContainer;

        public override PipeType AllowedPipeType => PipeType.Fluid;

        public static TransferFluidPort CreatePort(GameObject prefab, GameObject root, Vector3 position, Quaternion rotation, PortType type, bool outside = false)
        {
            return CreatePort<TransferFluidPort>(prefab, root, position, rotation, type, outside);
        }

        public override void CreateRepresentation(GameObject prefab, BaseModuleProvider provider)
        {
            representation = PhysicalFluidPortRepresentation.Create(prefab, this, provider, gameObject);
        }

        public override void EnsureHandlerAndRegister(GameObject prefab, BaseModuleProvider provider)
        {
            handler = prefab.EnsureComponent<TransferFluidPortHandler>();
            handler.Register(this);
            handler.WithBaseModule(provider);
        }

        public override void OnHoverStart()
        {
            representation.OnHoverStart();
        }

        public override void OnHover()
        {
            representation.OnHover();
        }

        public override void OnHoverEnd()
        {
            if (lockHover)
            {
                return;
            }

            representation.OnHoverEnd();
        }

        public override void Start()
        {
            base.Start();
            fluidContainer = gameObject.GetComponentInParent<ContainerProvider<FluidStack>>().Container;
        }

        public override bool TryExtract(NetworkFilter<FluidStack> filter, out FluidStack value, bool simulate = false)
        {
            if (!IsOutput)
            {
                value = default;
                return false;
            }

            return fluidContainer.TryExtract(filter, out value, simulate);
        }

        public override bool TryInsert(FluidStack value, bool simulate = false)
        {
            if (!IsInput)
            {
                return false;
            }

            return fluidContainer.TryInsert(value, simulate);
        }
    }
}
