using Industrica.Item.Network.Placed;
using System.Collections;

namespace Industrica.Network.Physical.Item
{
    public class ItemTransferPipe : TransferPipe<Pickupable>
    {
        public override PipeType Type => PipeType.Item;
        public override IEnumerator CreatePipe(PhysicalNetworkPort<Pickupable> start, PhysicalNetworkPort<Pickupable> end)
        {
            return CreatePipe<PlacedItemTransferPipe>(PlacedItemTransferPipe.Info.TechType, start, end);
        }
    }
}
