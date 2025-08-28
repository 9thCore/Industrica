using Industrica.ClassBase.Addons.Machine;
using Industrica.Utility;
using UWE;

namespace Industrica.ClassBase
{
    public abstract class BaseMachine : BaseConstructable
    {
        protected PowerRelay powerRelay;

        public override void Start()
        {
            base.Start();

            if (this is IExternalModule)
            {
                return;
            }

            powerRelay = GetComponentInParent<PowerRelay>();
            if (powerRelay != null)
            {
                OnPowerRelayFind(powerRelay);
            }
        }

        public virtual void OnEnable()
        {
            if (this is IExternalModule externalModule)
            {
                UpdateSchedulerUtils.Register(externalModule);
            }
        }

        public virtual void OnDisable()
        {
            if (this is IExternalModule externalModule)
            {
                UpdateSchedulerUtils.Deregister(externalModule);
            }
        }

        private void OnPowerRelayFind(PowerRelay powerRelay)
        {
            this.powerRelay = powerRelay;

            if (this is IRelayPowerChangeListener listener)
            {
                powerRelay.powerUpEvent.AddHandler(gameObject, new Event<PowerRelay>.HandleFunction(listener.PowerUpEvent));
                powerRelay.powerDownEvent.AddHandler(gameObject, new Event<PowerRelay>.HandleFunction(listener.PowerDownEvent));
            }

            if (this is IExternalModule externalModule)
            {
                externalModule.PowerFX.SetTarget(powerRelay.gameObject);
            }
        }

        public bool IsConnected()
        {
            return powerRelay != null;
        }

        public bool IsPowered()
        {
            if (!GameModeUtils.RequiresPower())
            {
                return true;
            }

            return powerRelay != null && powerRelay.IsPowered();
        }

        public bool HasEnergy(float energy)
        {
            if (!GameModeUtils.RequiresPower())
            {
                return true;
            }

            return powerRelay != null && powerRelay.GetPower() >= energy;
        }

        public bool TryConsumeEnergy(float energy)
        {
            if (!HasEnergy(energy))
            {
                return false;
            }

            powerRelay.ConsumeEnergy(energy, out _);
            return true;
        }

        public bool ConsumeEnergy(float energy, out float consumed)
        {
            if (!GameModeUtils.RequiresPower())
            {
                consumed = default;
                return true;
            }

            if (powerRelay == null)
            {
                consumed = default;
                return false;
            }

            return powerRelay.ConsumeEnergy(energy, out consumed);
        }

        public bool ConsumeEnergyPerSecond(float energy, out float consumed)
        {
            return ConsumeEnergy(energy * DayNightCycle.main.deltaTime, out consumed);
        }

        // These two implement IScheduledUpdateBehaviour,
        // if the machine extends it.
        // (IExternalModule extends from IScheduledUpdateBehaviour)
        public void ScheduledUpdate()
        {
            if (powerRelay != null
                || !isActiveAndEnabled)
            {
                return;
            }

            IExternalModule externalModule = (IExternalModule)this;

            SNUtil.GetNearestValidPowerRelay(
                position: transform.position,
                range: externalModule.PowerRelaySearchRange,
                validator: externalModule.PowerRelayValidator,
                callback: OnPowerRelayFind);
        }

        public string GetProfileTag()
        {
            return "Industrica_BaseMachinePowerRelayLookup";
        }

        // These three implement IConstructable,
        // if the machine extends it.
        // (IExternalModule extends from IConstructable)
        public virtual void OnConstructedChanged(bool constructed)
        {
            if (IsConnected()
                && this is IExternalModule externalModule)
            {
                externalModule.PowerFX.SetVFXVisible(constructed);
            }
        }

        public virtual bool IsDeconstructionObstacle()
        {
            return true;
        }

        public virtual bool CanDeconstruct(out string reason)
        {
            reason = default;
            return true;
        }
    }
}
