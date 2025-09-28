using Industrica.ClassBase;
using Industrica.Network.Container;
using Industrica.Network.Filter;
using Industrica.Network.Wire;
using Industrica.Save;
using Nautilus.Extensions;
using System.Linq;
using UnityEngine;

namespace Industrica.Network.Pipe
{
    public abstract class TransferPump<T> : BaseMachine, ContainerUpdateEvent<T>.ISubscriber where T : class
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

        private NetworkFilter<T> insertFilter = null;
        private float pumpTimeRemaining = 0f;
        private Container<T> inputContainer;
        private Container<T> outputContainer;
        private int doPumpAt = -1;

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

            handTarget.onHandHover = new HandTargetEvent();
            handTarget.onHandHover.AddListener(OnHover);

            Input.RegisterSubscriber(new InputSubscriber(this));
            Output.RegisterSubscriber(new OutputSubscriber(this));

            port.OnCharge += OnCharge;

            if (Input.connectedPort != null)
            {
                OnInputConnect();
            }

            if (Output.connectedPort != null)
            {
                OnOutputConnect();
            }
        }

        private void OnCharge()
        {
            if (Enabled())
            {
                OnContainerUpdate(null);
            }
        }

        public void Update()
        {
            if (doPumpAt == Time.frameCount)
            {
                TryPump();
                doPumpAt = -1;
                return;
            }

            if (pumpTimeRemaining > 0f)
            {
                pumpTimeRemaining -= DayNightCycle.main.deltaTime;

                if (pumpTimeRemaining <= 0f)
                {
                    TryPump();
                }
            }
        }

        public void OnDestroy()
        {
            InvalidateSave();
        }

        private void TryPump()
        {
            if (!ReadyToPump())
            {
                return;
            }

            if (TryConsumeEnergy(PumpEnergyUsage)
                && Input.connectedPort.TryExtract(insertFilter, out T value))
            {
                Output.connectedPort.TryInsert(value);
                pumpTimeRemaining = PumpInterval;
            }
        }

        private void OnEitherConnect()
        {
            TryPump();
        }

        public void OnInputConnect()
        {
            inputContainer = Input.connectedPort.Container;
            inputContainer.inputEvent.Register(this);

            OnEitherConnect();
        }

        public void OnOutputConnect()
        {
            outputContainer = Output.connectedPort.Container;
            outputContainer.outputEvent.Register(this);

            insertFilter = new InsertableNetworkFilter<T>(outputContainer);

            OnEitherConnect();
        }

        private void OnEitherDisconnect()
        {
            pumpTimeRemaining = 0f;
        }

        public void OnInputDisconnect()
        {
            if (Input.connectedPort == null
                && inputContainer != null)
            {
                inputContainer.inputEvent.Unregister(this);
                inputContainer = null;
            }

            OnEitherDisconnect();
        }

        public void OnOutputDisconnect()
        {
            if (Output.connectedPort == null
                && outputContainer != null)
            {
                outputContainer.outputEvent.Unregister(this);
                outputContainer = null;
            }

            OnEitherDisconnect();
        }

        public void OnContainerUpdate(Container<T> _)
        {
            doPumpAt = Time.frameCount + 1;
        }

        public bool ReadyToPump()
        {
            return Enabled()
                && pumpTimeRemaining <= 0f
                && Input.connectedPort != null
                && Output.connectedPort != null;
        }

        private bool Enabled()
        {
            if (!port.Occupied)
            {
                return true;
            }

            return port.value != WirePort.WireDefault;
        }

        private void OnHover(HandTargetEventData data)
        {
            string state = Enabled() switch
            {
                true => "Enabled",
                false => "Disabled"
            };

            HandReticle.main.SetText(HandReticle.TextType.Hand, $"IndustricaPump_Pumping{state}", true);
        }

        public const float PumpInterval = 1f;
        public const float PumpEnergyUsage = 0.02f;

        public abstract class BaseSaveData<S, C> : ComponentSaveData<S, C> where S : BaseSaveData<S, C> where C : TransferPump<T>
        {
            public float pumpTimeRemaining;

            protected BaseSaveData(C component) : base(component) { }

            public override void CopyFromStorage(S data)
            {
                pumpTimeRemaining = data.pumpTimeRemaining;
            }

            public override void Load()
            {
                Component.pumpTimeRemaining = pumpTimeRemaining;
            }

            public override void Save()
            {
                pumpTimeRemaining = Component.pumpTimeRemaining;
            }
        }

        private abstract record Subscriber(TransferPump<T> Holder) : TransferPort<T>.ISubscriber
        {
            public abstract void OnConnect();
            public abstract void OnDisconnect();
        }

        private record InputSubscriber(TransferPump<T> Holder) : Subscriber(Holder)
        {
            public override void OnConnect()
            {
                Holder.OnInputConnect();
            }

            public override void OnDisconnect()
            {
                Holder.OnInputDisconnect();
            }
        }

        private record OutputSubscriber(TransferPump<T> Holder) : Subscriber(Holder)
        {
            public override void OnConnect()
            {
                Holder.OnOutputConnect();
            }

            public override void OnDisconnect()
            {
                Holder.OnOutputDisconnect();
            }
        }
    }
}
