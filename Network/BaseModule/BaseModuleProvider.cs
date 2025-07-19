using UnityEngine;

namespace Industrica.Network.BaseModule
{
    public abstract class BaseModuleProvider : MonoBehaviour
    {
        public abstract float ConstructedAmount { get; }
        public abstract bool TryAddHandler(PortHandler handler, out PortHandler result);
    }

    public abstract class BaseModuleProvider<T> : BaseModuleProvider where T : MonoBehaviour, IBaseModule
    {
        private T module;
        protected T Module => module ??= GetComponent<T>();
    }
}
