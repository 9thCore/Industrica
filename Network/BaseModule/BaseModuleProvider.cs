using UnityEngine;

namespace Industrica.Network.BaseModule
{
    public abstract class BaseModuleProvider : MonoBehaviour
    {
        public PortHandler portHandler;

        public void SetPortHandler(PortHandler portHandler)
        {
            this.portHandler = portHandler;
        }

        public bool CanDeconstruct(out string reason)
        {
            if (portHandler == null)
            {
                reason = default;
                return true;
            }

            return portHandler.CanDeconstruct(out reason);
        }

        public abstract float ConstructedAmount { get; }
    }

    public abstract class BaseModuleProvider<T> : BaseModuleProvider where T : MonoBehaviour, IBaseModule
    {
        public T module;

        public BaseModuleProvider<T> WithModule(T module)
        {
            this.module = module;
            return this;
        }
    }
}
