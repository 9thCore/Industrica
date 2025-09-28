using Industrica.Network.Filter;
using Industrica.Network.Pipe;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public class PassthroughContainer<T> : Container<T> where T : class
    {
        private Container<T> inputContainer, outputContainer;
        private ContainerSubscriber inputSubscriber, outputSubscriber;
        public TransferPort<T> input, output;

        public PassthroughContainer(TransferPort<T> input, TransferPort<T> output)
        {
            this.input = input;
            this.output = output;

            inputSubscriber = new InputContainerSubscriber(this);
            outputSubscriber = new OutputContainerSubscriber(this);

            input.RegisterSubscriber(new InputPortSubscriber(this));
            output.RegisterSubscriber(new OutputPortSubscriber(this));
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

        private void OnInputConnect()
        {
            inputContainer = input.connectedPort.Container;
            inputContainer.inputEvent.Register(inputSubscriber);
        }

        private void OnInputDisconnect()
        {
            if (input.connectedPort == null
                && inputContainer != null)
            {
                inputContainer.inputEvent.Unregister(inputSubscriber);
                inputContainer = null;
            }
        }

        private void OnOutputConnect()
        {
            outputContainer = output.connectedPort.Container;
            outputContainer.outputEvent.Register(outputSubscriber);
        }

        private void OnOutputDisconnect()
        {
            if (output.connectedPort == null
                && outputContainer != null)
            {
                outputContainer.outputEvent.Unregister(outputSubscriber);
                outputContainer = null;
            }
        }

        private abstract record ContainerSubscriber(PassthroughContainer<T> Holder) : ContainerUpdateEvent<T>.ISubscriber
        {
            public abstract void OnContainerUpdate(Container<T> container);
        }

        private record InputContainerSubscriber(PassthroughContainer<T> Holder) : ContainerSubscriber(Holder)
        {
            public override void OnContainerUpdate(Container<T> container)
            {
                Holder.inputEvent.Raise(container);
            }
        }

        private record OutputContainerSubscriber(PassthroughContainer<T> Holder) : ContainerSubscriber(Holder)
        {
            public override void OnContainerUpdate(Container<T> container)
            {
                Holder.outputEvent.Raise(container);
            }
        }

        private abstract record PortSubscriber(PassthroughContainer<T> Holder) : TransferPort<T>.ISubscriber
        {
            public abstract void OnConnect();
            public abstract void OnDisconnect();
        }

        private record InputPortSubscriber(PassthroughContainer<T> Holder) : PortSubscriber(Holder)
        {
            public override void OnConnect()
            {
                Holder.OnInputConnect();
            }

            public override void OnDisconnect()
            {
                Holder.OnInputDisconnect();
            }
        }

        private record OutputPortSubscriber(PassthroughContainer<T> Holder) : PortSubscriber(Holder)
        {
            public override void OnConnect()
            {
                Holder.OnOutputConnect();
            }

            public override void OnDisconnect()
            {
                Holder.OnOutputDisconnect();
            }
        }
    }
}
