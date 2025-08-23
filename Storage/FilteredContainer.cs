using Industrica.ClassBase;
using Industrica.Network.Filter;

namespace Industrica.Storage
{
    public abstract class FilteredContainer<T> : BaseMachine where T : class
    {
        private NetworkFilter<T> filter;

        public void SetNetworkFilter(NetworkFilter<T> filter)
        {
            this.filter = filter;
        }

        public bool AllowedToAdd(T value)
        {
            if (!TryConsumeEnergy(EnergyUsagePerFilter))
            {
                // Allow anything through, if there is not enough power in the system
                return true;
            }

            return filter == null || filter.Matches(value);
        }

        public const float EnergyUsagePerFilter = 1f;
    }
}
