using Industrica.Network.Filter;
using System.Collections.Generic;

namespace Industrica.Network.Physical
{
    public class PhysicalNetworkPortPump<T> where T : class
    {
        private readonly PhysicalNetworkPort<T> parent;

        public NetworkFilter<T> insertFilter, extractFilter = null;
        public IEnumerable<PhysicalNetworkPort<T>> siblings, inverse = null;

        public PhysicalNetworkPortPump(PhysicalNetworkPort<T> port)
        {
            parent = port;
        }

        public void InputFromNetwork()
        {
            insertFilter ??= new InsertableNetworkFilter<T>(parent.Container);
            if (!parent.network.TryExtract(insertFilter, out T value))
            {
                return;
            }

            parent.Container.TryInsert(value);
        }

        public void OutputIntoNetwork()
        {
            extractFilter ??= new InsertableNetworkFilter<T>(parent.connectedPort.Container);
            if (parent.Container.TryExtract(extractFilter, out T value))
            {
                parent.network.TryInsert(value);
            }
        }
    }
}
