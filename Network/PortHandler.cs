using Industrica.Network.BaseModule;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class PortHandler : MonoBehaviour, IConstructable
    {
        public BaseModuleProvider provider;

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

            reason = DeconstructionDeniedReason.Translate();
            return false;
        }

        public abstract string DeconstructionDeniedReason { get; }
        public abstract bool CanDeconstructPorts();
        public abstract PortHandler CopyTo(GameObject prefab);
    }
}
