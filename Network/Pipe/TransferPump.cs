using Industrica.ClassBase;
using Industrica.Network.Container;
using Industrica.Network.Filter;
using Industrica.Network.Wire;
using Industrica.Save;
using Nautilus.Extensions;
using System.Linq;

namespace Industrica.Network.Pipe
{
    public abstract class TransferPump<T> : BaseMachine where T : class
    {
        private PassthroughContainer<T> container;
        public PassthroughContainer<T> Container => container ??= new PassthroughContainer<T>(Input, Output);

        private TransferPort<T> _output;
        public TransferPort<T> Output => _output.Exists() ?? (_output = GetComponentsInChildren<TransferPort<T>>()
            .Where(p => p.IsOutput)
            .First());

        private TransferPort<T> _input;
        public TransferPort<T> Input => _input.Exists() ?? (_input = GetComponentsInChildren<TransferPort<T>>()
            .Where(p => p.IsInput)
            .First());

        public abstract void CreateSave();
        public abstract void InvalidateSave();

        public bool enabledPump = true;
        private NetworkFilter<T> insertFilter = null;
        private float elapsedSinceLastPump = 0f;
        // Because we want the pump to pump at the earliest possible moment, if it becomes possible to pump "midway" through a pumping interval
        private bool queuedPump = false;

        public GenericHandTarget handTarget;
        public WirePort port;

        public TransferPump<T> WithHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public TransferPump<T> WithWirePort(WirePort port)
        {
            this.port = port;
            return this;
        }

        public override void Start()
        {
            base.Start();
            CreateSave();

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

        public void OnDestroy()
        {
            InvalidateSave();
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
                || Input.connectedPort == null
                || Output.connectedPort == null)
            {
                return;
            }

            if (TryConsumeEnergy(PumpEnergyUsage))
            {
                Pump();
            }

            queuedPump = false;
        }

        private void Pump()
        {
            insertFilter ??= new InsertableNetworkFilter<T>(Output.connectedPort.Container);
            if (Input.connectedPort.TryExtract(insertFilter, out T value))
            {
                Output.connectedPort.TryInsert(value);
            }
        }

        public bool Enabled()
        {
            if (DisabledByWire())
            {
                return false;
            }

            return enabledPump;
        }

        private bool DisabledByWire()
        {
            if (!port.Occupied)
            {
                return false;
            }

            return port.value == WirePort.WireDefault;
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

            if (DisabledByWire())
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "IndustricaWire_DisabledByWire", true);
            }
        }

        public const float PumpInterval = 5f;
        public const float PumpEnergyUsage = 1f;

        public abstract class BaseSaveData<S, C> : ComponentSaveData<S, C> where S : BaseSaveData<S, C> where C : TransferPump<T>
        {
            public float elapsedSinceLastPump;
            public bool enabledPump;

            protected BaseSaveData(C component) : base(component) { }

            public override void CopyFromStorage(S data)
            {
                elapsedSinceLastPump = data.elapsedSinceLastPump;
                enabledPump = data.enabledPump;
            }

            public override void Load()
            {
                Component.elapsedSinceLastPump = elapsedSinceLastPump;
                Component.enabledPump = enabledPump;
            }

            public override void Save()
            {
                elapsedSinceLastPump = Component.elapsedSinceLastPump;
                enabledPump = Component.enabledPump;
            }
        }
    }
}
