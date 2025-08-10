using Industrica.Network.Filter;
using Industrica.Network.Physical;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Industrica.Network.Container
{
    public class PumpContainer<T> : Container<T> where T : class
    {
        public PhysicalNetworkPort<T> input, output;

        public PumpContainer(PhysicalNetworkPort<T> input, PhysicalNetworkPort<T> output)
        {
            this.input = input;
            this.output = output;
        }

        protected override void Add(T value) { }

        public override int Count(NetworkFilter<T> filter)
        {
            int sum = 0;

            if (input.connectedPort != null)
            {
                sum += input.connectedPort.Container.Count(filter);
            }

            if (output.connectedPort != null)
            {
                sum += output.connectedPort.Container.Count(filter);
            }

            return sum;
        }

        public override int CountRemovable(NetworkFilter<T> filter)
        {
            if (input.connectedPort == null)
            {
                return 0;
            }

            return input.connectedPort.Container.CountRemovable(filter);
        }


        public override IEnumerator<T> GetEnumerator()
        {
            if (input.connectedPort != null)
            {
                foreach (T value in input.connectedPort.Container)
                {
                    yield return value;
                }
            }

            if (output.connectedPort != null)
            {
                foreach (T value in output.connectedPort.Container)
                {
                    yield return value;
                }
            }
        }

        protected override void Remove(T value) { }

        public override bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false)
        {
            if (input.connectedPort == null)
            {
                value = default;
                return false;
            }

            return input.connectedPort.Container.TryExtract(filter, out value, simulate);
        }

        public override bool TryInsert(T value, bool simulate = false)
        {
            if (output.connectedPort == null)
            {
                return false;
            }

            return output.connectedPort.Container.TryInsert(value, simulate);
        }
    }
}
