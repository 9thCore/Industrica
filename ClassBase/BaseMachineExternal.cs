using Industrica.ClassBase.Addons.Machine;
using Nautilus.Extensions;

namespace Industrica.ClassBase
{
    public abstract class BaseMachineExternal : BaseMachine, IExternalModule
    {
        private PowerFX powerFX;
        private int _scheduledUpdateIndex;

        public PowerFX PowerFX => powerFX.Exists() ?? (powerFX = GetComponentInChildren<PowerFX>(true));
        public int scheduledUpdateIndex { get => _scheduledUpdateIndex; set => _scheduledUpdateIndex = value; }

        public virtual float PowerRelaySearchRange => DefaultSearchRange;
        public virtual bool PowerRelayValidator(PowerRelay powerRelay) => true;

        public const float DefaultSearchRange = 20f;
    }
}
