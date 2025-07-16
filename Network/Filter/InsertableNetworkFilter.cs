using Industrica.Network.Container;

namespace Industrica.Network.Filter
{
    public class InsertableNetworkFilter<T> : NetworkFilter<T>
    {
        private readonly Container<T> container;

        public InsertableNetworkFilter(Container<T> container)
        {
            this.container = container;
        }

        public override bool Matches(T value)
        {
            return container.TryInsert(value, true);
        }
    }
}
