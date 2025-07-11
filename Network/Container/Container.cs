using Industrica.Network.Filter;
using System.Collections.Generic;

namespace Industrica.Network.Container
{
    public abstract class Container<T>
    {
        public abstract bool TryInsert(T value);
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value);
        public abstract IEnumerator<T> GetEnumerator();

    }
}
