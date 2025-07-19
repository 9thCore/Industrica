using Industrica.Network.BaseModule;
using System.Collections;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class PortHandler : MonoBehaviour, IConstructable
    {
        public BaseModuleProvider provider;
        private bool keepTryingToAddGeometryProvider = false;
        private bool queuedGeometryPatch = false;
        private int count = 0;
        private PortHandler geometryHandler;

        public void WithBaseModule(BaseModuleProvider provider)
        {
            this.provider = provider;
        }

        public void Start()
        {
            if (provider != null)
            {
                keepTryingToAddGeometryProvider = provider.TryAddHandler(this, out geometryHandler);
            }
        }

        public void Update()
        {
            // Refresh the geometry's component if it gets removed (module deconstructed), but wait until it's constructed again
            if (!keepTryingToAddGeometryProvider
                || provider == null
                || geometryHandler != null)
            {
                return;
            }

            if (queuedGeometryPatch
                && provider.ConstructedAmount >= 1f)
            {
                keepTryingToAddGeometryProvider = provider.TryAddHandler(this, out geometryHandler);
                queuedGeometryPatch = false;
                return;
            }

            // Introduce a one-frame delay
            queuedGeometryPatch = true;
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

            reason = Language.main.Get("IndustricaPort_CannotDeconstructConnected");
            return false;
        }

        public abstract bool CanDeconstructPorts();
        public abstract PortHandler CopyTo(GameObject prefab);
    }
}
