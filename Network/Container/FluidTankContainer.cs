using Industrica.Fluid;
using Industrica.Network.Filter;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public class FluidTankContainer : FluidContainer, FluidTank.ISubscriber
    {
        private readonly FluidTank tank;

        public FluidTankContainer(FluidTank tank)
        {
            this.tank = tank;
            tank.Subscribe(this);
        }

        public override int GetAvailableFluidSpace()
        {
            if (tank.Empty())
            {
                return tank.maxAmount;
            }

            return tank.maxAmount - tank.fluidStack.amount;
        }

        public override int Count(NetworkFilter<FluidStack> filter)
        {
            return filter.Matches(tank.fluidStack) ? 1 : 0;
        }

        public override int CountRemovable(NetworkFilter<FluidStack> filter)
        {
            return filter.Matches(tank.fluidStack) && tank.AllowedToRemove() ? 1 : 0;
        }

        public override IEnumerator<FluidStack> GetEnumerator()
        {
            yield return tank.fluidStack;
        }

        public override bool TryExtract(NetworkFilter<FluidStack> filter, out FluidStack value, bool simulate = false)
        {
            if (tank.Empty()
                || !tank.AllowedToRemove()
                || !filter.Matches(tank.fluidStack))
            {
                value = default;
                return false;
            }

            value = tank.Split(filter.fluidAmount, simulate);
            return true;
        }

        public override bool TryInsert(FluidStack value, bool simulate = false)
        {
            if (!tank.AllowedToAdd())
            {
                return false;
            }

            if (tank.Empty())
            {
                if (!simulate)
                {
                    tank.SetFluidStack(value);
                }
                
                return true;
            }

            if (tank.fluidStack.techType != value.techType)
            {
                return false;
            }

            tank.Add(value.amount, out int _, simulate);
            return true;
        }

        protected override void Add(FluidStack value)
        {
            TryInsert(value);
        }

        protected override void Remove(FluidStack value)
        {
            if (!tank.AllowedToRemove())
            {
                return;
            }

            if (tank.Empty()
                || tank.fluidStack.techType != value.techType)
            {
                return;
            }

            tank.Remove(value.amount, out _);
        }

        public void OnFluidInput()
        {
            inputEvent.Raise(this);
        }

        public void OnFluidOutput()
        {
            outputEvent.Raise(this);
        }
    }
}
