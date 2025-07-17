using UnityEngine;

namespace Industrica.Network.Container.Provider
{
    public abstract class ContainerProvider<T> : MonoBehaviour where T : class
    {
        public abstract Container<T> Container { get; }
    }
}
