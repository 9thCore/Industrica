using Industrica.Fluid;
using System.Collections;
using UnityEngine;

namespace Industrica.Network.Pipe.Fluid
{
    internal class FluidTransferPipe : TransferPipe<FluidStack>
    {
        public override PipeType Type => PipeType.Fluid;
        public override Vector3 Scale => Vector3.one * 2f;
        public override Color StretchedPartColor => FluidPipeColor;
        public override Color BendColor => FluidPipeBendColor;
        public override string UseText => "Use_IndustricaFluidPipe";

        public override IEnumerator CreatePipe(TransferPort<FluidStack> start, TransferPort<FluidStack> end)
        {
            return CreatePipe<PlacedFluidTransferPipe>(PlacedFluidTransferPipe.Info.TechType, start, end);
        }

        public static readonly Color FluidPipeColor = Color.cyan;
        public static readonly Color FluidPipeBendColor = new Color32(225, 224, 222, 255);
    }
}
