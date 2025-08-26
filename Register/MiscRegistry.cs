using Industrica.Network;
using Industrica.Network.Physical.Item;
using Industrica.Network.Systems;
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

            ItemPhysicalNetwork.RegisterPrefab();
        }
    }
}
