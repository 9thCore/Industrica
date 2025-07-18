using Industrica.Network.Filter;
using Industrica.Network.Systems;
using System.Collections.Generic;

namespace Industrica.Network.Physical
{
    public class PhysicalNetworkPortPump<T> where T : class
    {
        private readonly PhysicalNetworkPort<T> parent;

        public PhysicalNetworkPortHandler<T> handler;
        public NetworkFilter<T> insertFilter, extractFilter = null;
        public IEnumerable<PhysicalNetworkPort<T>> siblings, inverse = null;

        public PhysicalNetworkPortPump(PhysicalNetworkPort<T> port, PhysicalNetworkPortHandler<T> handler)
        {
            parent = port;
            this.handler = handler;
        }

        public void SetNetwork(PhysicalNetwork<T> network)
        {
            network.OnPump += UpdateAuto;
        }

        public void NetworkDisconnect()
        {
            if (parent.network != null)
            {
                parent.network.OnPump -= UpdateAuto;
            }
        }

        public void UpdateAuto()
        {
            InputFromNetwork();
            OutputIntoNetwork();
        }

        public void InputFromNetwork()
        {
            if (parent.network == null
                || parent.port != PortType.Input)
            {
                return;
            }

            insertFilter ??= new InsertableNetworkFilter<T>(parent.Container);
            if (!parent.network.TryExtract(insertFilter, out T value))
            {
                return;
            }

            parent.Container.TryInsert(value);
        }

        public void OutputIntoNetwork()
        {
            if (parent.network == null
                || parent.port != PortType.Output)
            {
                return;
            }

            extractFilter ??= new InsertableNetworkFilter<T>(parent.connectedPort.Container);
            if (parent.Container.TryExtract(extractFilter, out T value))
            {
                parent.network.TryInsert(value);
            }
        }
    }
}
