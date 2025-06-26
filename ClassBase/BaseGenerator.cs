namespace Industrica.ClassBase
{
    public abstract class BaseGenerator : BaseMachine
    {
        protected PowerSource powerSource;

        public abstract bool CanGenerate { get; }
        public abstract float MaxPower { get; }
        public int MaxPowerInt => (int)MaxPower;
        public float Power => powerSource.power;
        public float PowerInt => (int)Power;

        public override void Start()
        {
            base.Start();

            powerSource = gameObject.EnsureComponent<PowerSource>();
            powerSource.maxPower = MaxPower;
        }

        public void SetPower(float power)
        {
            powerSource.SetPower(power);
        }

        public bool GeneratePower(float power, bool force, out float consumed)
        {
            if (!CanGenerate && !force)
            {
                consumed = default;
                return false;
            }

            float available = powerSource.maxPower - powerSource.power;

            if (available > 0)
            {
                if (available < power)
                {
                    power = available;
                }

                return powerSource.AddEnergy(power, out consumed);
            }

            consumed = default;
            return false;
        }

        public bool GeneratePowerPerSecond(float power, bool force, out float consumed)
        {
            return GeneratePower(power * DayNightCycle.main.deltaTime, force, out consumed);
        }
    }
}
