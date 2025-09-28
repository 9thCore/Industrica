using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public class ContainerUpdateEvent<T> where T : class
    {
        private readonly List<ISubscriber> subscribers = new();

        public void Register(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void Unregister(ISubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public void Raise(Container<T> instance)
        {
            for (int i = 0; i < subscribers.Count; i++)
            {
                subscribers[i].OnContainerUpdate(instance);
            }
        }

        public interface ISubscriber
        {
            public void OnContainerUpdate(Container<T> container);
        }
    }
}
