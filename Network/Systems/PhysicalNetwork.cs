using Industrica.Network.Container;
using Industrica.Network.Physical;

namespace Industrica.Network.Systems
{
    public class PhysicalNetwork<T> : Network<T, PhysicalNetwork<T>.PhysicalConnection>
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
    }
}
