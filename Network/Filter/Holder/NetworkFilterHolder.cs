using UnityEngine;

namespace Industrica.Network.Filter.Holder
{
    public abstract class NetworkFilterHolder<T> : MonoBehaviour
    {
        private NetworkFilter<T> filter;
        public NetworkFilter<T> Filter => filter ??= GetFilter;
        public abstract NetworkFilter<T> GetFilter { get; }
    }
}
