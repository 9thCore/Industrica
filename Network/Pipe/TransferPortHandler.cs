using System.Collections.Generic;
using System.Linq;

namespace Industrica.Network.Pipe
{
    public abstract class TransferPortHandler<T> : PortHandler where T : class
    {
        protected static bool CanDeconstruct(IEnumerable<TransferPort<T>> ports)
        {
            return ports == null || ports.All(c => c == null || c.connectedPort == null);
        }

        public abstract void Register(TransferPort<T> port);
    }
}
