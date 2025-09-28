using Industrica.Network.Filter;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public abstract class Container<T> where T : class
    {
        public readonly ContainerUpdateEvent<T> inputEvent = new();
        public readonly ContainerUpdateEvent<T> outputEvent = new();

        public abstract int Count(NetworkFilter<T> filter);
        public abstract int CountRemovable(NetworkFilter<T> filter);
        public abstract bool TryInsert(T value, bool simulate = false);
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false);
        protected abstract void Add(T value);
        protected abstract void Remove(T value);
        public abstract IEnumerator<T> GetEnumerator();

        public int Count()
        {
            return Count(AlwaysNetworkFilter<T>.Instance);
        }

        public int CountRemovable()
        {
            return CountRemovable(AlwaysNetworkFilter<T>.Instance);
        }

        public void RegisterInputSubscriber(ContainerUpdateEvent<T>.ISubscriber subscriber)
        {
            inputEvent.Register(subscriber);
        }

        public void RegisterOutputSubscriber(ContainerUpdateEvent<T>.ISubscriber subscriber)
        {
            outputEvent.Register(subscriber);
        }

        public void UnregisterInputSubscriber(ContainerUpdateEvent<T>.ISubscriber subscriber)
        {
            inputEvent.Unregister(subscriber);
        }

        public void UnregisterOutputSubscriber(ContainerUpdateEvent<T>.ISubscriber subscriber)
        {
            outputEvent.Unregister(subscriber);
        }

        public void RegisterSubscriber(ContainerUpdateEvent<T>.ISubscriber subscriber)
        {
            RegisterInputSubscriber(subscriber);
            RegisterOutputSubscriber(subscriber);
        }

        public void UnregisterSubscriber(ContainerUpdateEvent<T>.ISubscriber subscriber)
        {
            UnregisterInputSubscriber(subscriber);
            UnregisterOutputSubscriber(subscriber);
        }
    }
}
