using Industrica.Network.Physical;

namespace Industrica.Container
{
    public abstract class PumpSlot<T> where T : class
    {
        public readonly PhysicalNetworkPort<T> input, output;

        public delegate void SlotUpdate(T value);
        public event SlotUpdate OnSlotInput;

        public PumpSlot(PhysicalNetworkPort<T> input, PhysicalNetworkPort<T> output)
        {
            this.input = input;
            this.output = output;
        }

        public void InputSlot(T value)
        {
            OnSlotInput?.Invoke(value);
        }

        public P WithSubscriber<P>(SlotUpdate subscriber) where P : PumpSlot<T>
        {
            OnSlotInput += subscriber;
            return this as P;
        }
    }
}
