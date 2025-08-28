namespace Industrica.ClassBase.Addons.Machine
{
    public interface IRelayPowerChangeListener
    {
        public void PowerUpEvent(PowerRelay relay);
        public void PowerDownEvent(PowerRelay relay);
    }
}
