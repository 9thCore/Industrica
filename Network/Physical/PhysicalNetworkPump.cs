using Industrica.Container;
using Industrica.Utility;
using Nautilus.Extensions;
using System.Linq;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPump<T, P> : MonoBehaviour where P : PumpSlot<T>
    {
        private static Texture _texture = null;
        public static Texture Texture => _texture ??= PathUtil.GetTexture("Pump/monitor");

        private GameObject storageRoot;
        public GameObject StorageRoot => storageRoot.Exists() ?? (storageRoot = GameObjectUtil.CreateChild(gameObject, nameof(storageRoot)));

        private P storage;
        public P Storage => storage ??= GetStorage.WithSubscriber<P>(OnInput);
        public abstract P GetStorage { get; }

        private PhysicalNetworkPort<T> _output;
        public PhysicalNetworkPort<T> Output => _output.Exists() ?? (_output = GetComponentsInChildren<PhysicalNetworkPort<T>>()
            .Where(p => p.IsOutput)
            .First());

        private PhysicalNetworkPort<T> _input;
        public PhysicalNetworkPort<T> Input => _input.Exists() ?? (_input = GetComponentsInChildren<PhysicalNetworkPort<T>>()
            .Where(p => p.IsInput)
            .First());

        private float elapsedSinceLastPump = 0f;

        public void Update()
        {
            elapsedSinceLastPump += DayNightCycle.main.deltaTime;
            if (elapsedSinceLastPump < PumpInterval)
            {
                return;
            }

            elapsedSinceLastPump -= PumpInterval;

            
        }

        protected void OnInput(T value)
        {
            Output.ConnectedPort.TryInsert(value);
        }

        public const float PumpInterval = 5f;
    }
}
