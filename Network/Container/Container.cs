using Industrica.Network.Filter;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public abstract class Container<T>
    {
        public abstract int Count(NetworkFilter<T> filter);
        public abstract int CountRemovable(NetworkFilter<T> filter);
        public abstract bool TryInsert(T value, bool simulate = false);
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false);
        protected abstract void Add(T value);
        protected abstract void Remove(T value);
        public abstract IEnumerator<T> GetEnumerator();
    }
}
