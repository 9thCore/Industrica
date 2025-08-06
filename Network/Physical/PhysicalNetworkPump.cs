using Industrica.Container;
using Industrica.Network.Filter;
using Industrica.Utility;
using Nautilus.Extensions;
using System.Linq;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPump<T, P> : MonoBehaviour where P : PumpSlot<T> where T : class
    {
        private static Texture _texture = null;
        public static Texture Texture => _texture ??= PathUtil.GetTexture("Pump/monitor");

        private GameObject storageRoot;
        public GameObject StorageRoot => storageRoot.Exists() ?? (storageRoot = gameObject.CreateChild(nameof(storageRoot)));

        private P storage;
        public P Storage => storage ??= GetStorage;
        public abstract P GetStorage { get; }

        private PhysicalNetworkPort<T> _output;
        public PhysicalNetworkPort<T> Output => _output.Exists() ?? (_output = GetComponentsInChildren<PhysicalNetworkPort<T>>()
            .Where(p => p.IsOutput)
            .First());

        private PhysicalNetworkPort<T> _input;
        public PhysicalNetworkPort<T> Input => _input.Exists() ?? (_input = GetComponentsInChildren<PhysicalNetworkPort<T>>()
            .Where(p => p.IsInput)
            .First());

        public bool enabledPump = true;
        private NetworkFilter<T> insertFilter = null;
        private float elapsedSinceLastPump = 0f;
        // Because we want the pump to pump at the earliest possible moment, if it becomes possible to pump "midway" through a pumping interval
        private bool queuedPump = false;

        public GenericHandTarget handTarget;

        public PhysicalNetworkPump<T, P> WithHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public void Start()
        {
            if (!handTarget)
            {
                return;
            }

            handTarget.onHandClick = new HandTargetEvent();
            handTarget.onHandClick.AddListener(OnClick);
            handTarget.onHandHover = new HandTargetEvent();
            handTarget.onHandHover.AddListener(OnHover);
        }

        public void Update()
        {
            UpdateTimer();
            TryPump();
        }

        private void UpdateTimer()
        {
            elapsedSinceLastPump += DayNightCycle.main.deltaTime;
            if (elapsedSinceLastPump < PumpInterval)
            {
                return;
            }

            elapsedSinceLastPump -= PumpInterval;

            queuedPump = true;
        }

        private void TryPump()
        {
            if (!queuedPump
                || !Enabled()
                || !Input.HasNetwork
                || !Output.HasNetwork)
            {
                return;
            }

            Pump();
            queuedPump = false;
        }

        private void Pump()
        {
            insertFilter ??= new InsertableNetworkFilter<T>(Output.connectedPort.Container);
            if (Input.network.TryExtract(insertFilter, out T value))
            {
                Output.network.TryInsert(value);
            }
        }

        public bool Enabled()
        {
            return enabledPump;
        }

        private void OnClick(HandTargetEventData data)
        {
            enabledPump = !enabledPump;
        }

        private void OnHover(HandTargetEventData data)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand);

            string state = enabledPump switch
            {
                true => "Enabled",
                false => "Disabled"
            };

            HandReticle.main.SetText(HandReticle.TextType.Hand, $"IndustricaPump_Pumping{state}", true, GameInput.Button.LeftHand);
        }

        public const float PumpInterval = 5f;
    }
}
