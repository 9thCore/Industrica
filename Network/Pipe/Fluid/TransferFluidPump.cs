using Industrica.Fluid;
using Industrica.Network.Container;
using Industrica.Save;
using System;

namespace Industrica.Network.Pipe.Fluid
{
    public class TransferFluidPump : TransferPump<FluidStack>
    {
        private SaveData save;
        private FluidContainer fluidContainer;

        public override void CreateSave()
        {
            save = new(this);
        }

        public override void InvalidateSave()
        {
            save.Invalidate();
        }

        protected override void TryPump()
        {
            if (!ReadyToPump())
            {
                return;
            }

            if (fluidContainer != outputContainer)
            {
                fluidContainer = (FluidContainer)outputContainer;
            }

            insertFilter.fluidAmount = Math.Min(500, fluidContainer.GetAvailableFluidSpace());
            base.TryPump();
        }

        public class SaveData : BaseSaveData<SaveData, TransferFluidPump>
        {
            public SaveData(TransferFluidPump component) : base(component) { }
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.physicalFluidPumpSaveData;
        }
    }
}
