using Industrica.Network.Pipe.Item;
using Industrica.Network.Wire;

namespace Industrica.Network
{
    public static class BaseModConnectionTools
    {
        public static void Register()
        {
            MultiTool.RegisterConnectionTool<ItemTransferPipe>();
            MultiTool.RegisterConnectionTool<WireTool>();
        }
    }
}
