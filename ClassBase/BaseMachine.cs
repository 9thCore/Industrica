namespace Industrica.ClassBase
{
    public abstract class BaseMachine : BaseConstructable
    {
        protected PowerRelay powerRelay;

        public override void Start()
        {
            base.Start();

            powerRelay = GetComponentInParent<PowerRelay>();
            if (powerRelay == null)
            {
                Plugin.Logger.LogError($"Could not find {nameof(PowerRelay)} in tree of {gameObject.name}! Disabling...");
                enabled = false;
            }
        }

        public bool ConsumeEnergyExact(float energy)
        {
            float power = powerRelay.GetPower();
            if (power < energy)
            {
                return false;
            }

            powerRelay.ConsumeEnergy(energy, out _);
            return true;
        }

        public bool ConsumeEnergyGreedy(float energy, out float consumed)
        {
            return powerRelay.ConsumeEnergy(energy, out consumed);
        }
    }
}
