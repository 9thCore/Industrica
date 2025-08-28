namespace Industrica.ClassBase.Addons.Machine
{
    public interface IExternalModule : IScheduledUpdateBehaviour, IConstructable
    {
        public float PowerRelaySearchRange { get; }
        public PowerFX PowerFX { get; }
        public bool PowerRelayValidator(PowerRelay powerRelay);
    }
}
