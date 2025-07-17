using Industrica.Network.Container;
using Industrica.Network.Physical;
using System.Collections.Generic;

namespace Industrica.Network.Systems
{
    public class PhysicalNetwork<T> : Network<T, PhysicalNetwork<T>.PhysicalConnection, PhysicalNetwork<T>.SingleContainerWrapper> where T : class
    {
        public PhysicalConnection Register(PortType type, PhysicalNetworkPort<T> port)
        {
            return Register(type, new PhysicalConnection(this, type, port.Container, port));
        }

        public record PhysicalConnection(
            PhysicalNetwork<T> PhysicalNetwork,
            PortType Type,
            Container<T> Container,
            PhysicalNetworkPort<T> Port)
            : NetworkConnection(PhysicalNetwork, Type, Container);

        public class SingleContainerWrapper : ContainerWrapper<PhysicalConnection>
        {
            private PhysicalConnection connection;
            public override bool IsEmpty => connection == null;

            public override void Add(PhysicalConnection c)
            {
                connection = c;
            }

            public override IEnumerator<PhysicalConnection> GetEnumerator()
            {
                yield return connection;
            }

            public override void Remove(PhysicalConnection _)
            {
                connection = null;
            }
        }
    }
}
