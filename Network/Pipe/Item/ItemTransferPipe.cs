using System.Collections;
using UnityEngine;

namespace Industrica.Network.Pipe.Item
{
    public class ItemTransferPipe : TransferPipe<Pickupable>
    {
        public override PipeType Type => PipeType.Item;
        public override string UseText => "Use_IndustricaItemPipe";
        public override Vector3 Scale => Vector3.one;
        public override Color StretchedPartColor => ItemPipeColor;
        public override Color BendColor => ItemPipeBendColor;

        public override IEnumerator CreatePipe(TransferPort<Pickupable> start, TransferPort<Pickupable> end)
        {
            return CreatePipe<PlacedItemTransferPipe>(PlacedItemTransferPipe.Info.TechType, start, end);
        }

        public static readonly Color ItemPipeColor = Color.white;
        public static readonly Color ItemPipeBendColor = new Color32(225, 224, 222, 255);
    }
}
