using UnityEngine;

namespace Industrica.Network.BaseModule
{
    public abstract class BaseModuleConstructionProvider<T> : MonoBehaviour, IBaseModuleConstructionProvider where T : MonoBehaviour, IBaseModule
    {
        private T module;
        protected T Module => module ??= GetComponent<T>();
        public abstract float ConstructedAmount { get; }
    }
}
