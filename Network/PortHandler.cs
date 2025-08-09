using Industrica.Network.BaseModule;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class PortHandler : MonoBehaviour, IConstructable
    {
        public BaseModuleProvider provider;
        private int count = 0;

        public void WithBaseModule(BaseModuleProvider provider)
        {
            this.provider = provider;
        }

        public void Start()
        {
            if (provider != null)
            {
                provider.AddGeometryHandler(this);
            }
        }

        public string GetClassID()
        {
            return $"PhysicalNetworkPort{count++}";
        }

        public bool IsDeconstructionObstacle()
        {
            return true;
        }

        public void OnConstructedChanged(bool constructed)
        {

        }

        public bool CanDeconstruct(out string reason)
        {
            if (CanDeconstructPorts())
            {
                reason = default;
                return true;
            }

            reason = Language.main.Get(DeconstructionDeniedReason);
            return false;
        }

        public abstract string DeconstructionDeniedReason { get; }
        public abstract bool CanDeconstructPorts();
        public abstract PortHandler CopyTo(GameObject prefab);
    }
}
