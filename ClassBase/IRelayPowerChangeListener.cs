namespace Industrica.ClassBase
{
    public interface IRelayPowerChangeListener
    {
        public void PowerUpEvent(PowerRelay relay);
        public void PowerDownEvent(PowerRelay relay);
    }
}
