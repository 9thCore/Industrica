using UnityEngine;

namespace Industrica.Network.BaseModule
{
    public abstract class BaseModuleConstructionProvider : MonoBehaviour
    {
        public abstract float ConstructedAmount { get; }
    }

    public abstract class BaseModuleConstructionProvider<T> : BaseModuleConstructionProvider where T : MonoBehaviour, IBaseModule
    {
        private T module;
        protected T Module => module ??= GetComponent<T>();
    }
}
