using Industrica.Container;
using Industrica.Network.Filter;
using System;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public class PumpContainer<T> : Container<T>
    {
        private readonly PumpSlot<T> slot;

        public PumpContainer(PumpSlot<T> slot)
        {
            this.slot = slot;
        }

        public override bool CanInsert(T value)
        {
            if (slot.output.ConnectedPort == null)
            {
                return false;
            }

            return slot.output.ConnectedPort.Container.CanInsert(value);
        }

        public override bool Contains(NetworkFilter<T> filter, out bool canExtract)
        {
            if (slot.input.ConnectedPort == null)
            {
                canExtract = default;
                return false;
            }

            return slot.input.ConnectedPort.Container.Contains(filter, out canExtract);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        public override bool TryExtract(NetworkFilter<T> filter, out T value)
        {
            if (slot.input.ConnectedPort == null)
            {
                value = default;
                return false;
            }

            return slot.input.ConnectedPort.Container.TryExtract(filter, out value);
        }

        public override bool TryInsert(T value)
        {
            if (!CanInsert(value))
            {
                return false;
            }

            slot.InputSlot(value);
            return true;
        }
    }
}
