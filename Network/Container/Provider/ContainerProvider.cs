using UnityEngine;

namespace Industrica.Network.Container.Provider
{
    public abstract class ContainerProvider<T> : MonoBehaviour
    {
        public abstract Container<T> Container { get; }
    }
}
