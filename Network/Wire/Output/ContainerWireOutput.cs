using Industrica.ClassBase;
using Industrica.Network.Container;
using Industrica.Network.Container.Provider;
using UnityEngine;

namespace Industrica.Network.Wire.Output
{
    public abstract class ContainerWireOutput<T> : BaseMachine, IRelayPowerChangeListener where T : class
    {
        private Container<T> container;
        public WirePort port;

        public void SetPort(WirePort port)
        {
            this.port = port;
        }

        public void OnEnable()
        {
            if (container == null)
            {
                container = GetComponentInParent<ContainerProvider<T>>().Container;
            }
            
            container.OnUpdate += OnUpdate;
        }

        public void OnDisable()
        {
            container.OnUpdate -= OnUpdate;
        }

        private void OnUpdate(Container<T> container)
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
