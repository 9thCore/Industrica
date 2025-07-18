using System.Collections.Generic;
using System.Linq;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPortHandler<T> : PortHandler where T : class
    {
        protected static bool CanDeconstruct(IEnumerable<PhysicalNetworkPort<T>> ports)
        {
            return ports == null || ports.All(c => c == null || !c.HasNetwork);
        }

        public abstract void Register(PhysicalNetworkPort<T> port);
    }
}
