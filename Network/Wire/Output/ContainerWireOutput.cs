using Industrica.ClassBase;
using Industrica.ClassBase.Addons.Machine;
using Industrica.Network.Container;
using Industrica.Network.Container.Provider;

namespace Industrica.Network.Wire.Output
{
    public abstract class ContainerWireOutput<T> : BaseMachine, IRelayPowerChangeListener, ContainerUpdateEvent<T>.ISubscriber where T : class
    {
        private Container<T> container;
        public WirePort port;

        public void SetPort(WirePort port)
        {
            this.port = port;
        }

        public override void OnEnable()
        {
            if (container == null)
            {
                container = GetComponentInParent<ContainerProvider<T>>().Container;
            }

            container.RegisterSubscriber(this);
        }

        public override void OnDisable()
        {
            container.UnregisterSubscriber(this);
        }

        public void OnContainerUpdate(Container<T> container)
        {
            if (!IsPowered())
            {
                return;
            }

            port.SetElectricity(container.Count());
        }

        public void PowerUpEvent(PowerRelay relay)
        {
            port.SetElectricity(container.Count());
        }

        public void PowerDownEvent(PowerRelay relay)
        {
            port.SetElectricity(0);
        }
    }
}
