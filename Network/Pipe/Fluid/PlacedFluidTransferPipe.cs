using Industrica.Fluid;
using Industrica.Network.Pipe.Item;
using Industrica.Save;
using Nautilus.Assets;
using UnityEngine;

namespace Industrica.Network.Pipe.Fluid
{
    public class PlacedFluidTransferPipe : PlacedTransferPipe<FluidStack>
    {
        private SaveData save;
        public static PrefabInfo Info { get; private set; }

        public override Vector3 Scale => Vector3.one * 2f;
        public override Color StretchedPartColor => FluidTransferPipe.FluidPipeColor;
        public override Color BendColor => FluidTransferPipe.FluidPipeBendColor;

        public static void Register()
        {
            Info = Register<PlacedFluidTransferPipe>("IndustricaPlacedFluidTransferPipe");
        }

        protected override void CreateSave()
        {
            save = new(this);
        }

        protected override void InvalidateSave()
        {
            save.Invalidate();
        }

        public class SaveData : TransferPipeSaveData<SaveData, PlacedFluidTransferPipe>
        {
            public SaveData(PlacedFluidTransferPipe component) : base(component) { }
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.placedFluidTransferPipeData;
        }
    }
}
