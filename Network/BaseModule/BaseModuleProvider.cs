using UnityEngine;

namespace Industrica.Network.BaseModule
{
    public abstract class BaseModuleProvider : MonoBehaviour
    {
        public abstract float ConstructedAmount { get; }
        public abstract void AddGeometryHandler(PortHandler handler);
    }

    public abstract class BaseModuleProvider<T> : BaseModuleProvider where T : MonoBehaviour, IBaseModule
    {
        private T module;
        protected T Module => module ??= GetComponent<T>();
    }
}
