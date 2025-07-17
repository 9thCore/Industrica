using Industrica.Container;
using Industrica.Network.Filter;
using System;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public class PumpContainer<T> : Container<T> where T : class
    {
        private readonly PumpSlot<T> slot;

        public PumpContainer(PumpSlot<T> slot)
        {
            this.slot = slot;
        }

        protected override void Add(T value)
        {
            slot.InputSlot(value);
        }

        public override int Count(NetworkFilter<T> filter)
        {
            return 0;
        }

        public override int CountRemovable(NetworkFilter<T> filter)
        {
            return 0;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        protected override void Remove(T value)
        {
            
        }

        public override bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false)
        {
            if (slot.input.connectedPort == null)
            {
                value = default;
                return false;
            }

            return slot.input.connectedPort.Container.TryExtract(filter, out value, simulate);
        }

        public override bool TryInsert(T value, bool simulate = false)
        {
            if (slot.output.connectedPort == null)
            {
                return false;
            }

            if (slot.output.connectedPort.Container.TryInsert(value, true))
            {
                Add(value);
                return true;
            }

            return false;
        }
    }
}
