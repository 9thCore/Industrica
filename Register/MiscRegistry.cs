using Industrica.Network;
using Industrica.Network.Pipe.Item;
using Industrica.Network.Wire;
using Industrica.Operation;

namespace Industrica.Register
{
    public static class MiscRegistry
    {
        public static void Register()
        {
            BaseModOperations.Register();
            BaseModConnectionTools.Register();

            PlacedItemTransferPipe.Register();
            PlacedWire.Register();
        }
    }
}
