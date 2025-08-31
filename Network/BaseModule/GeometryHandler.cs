using UnityEngine;

namespace Industrica.Network.BaseModule
{
    public class GeometryHandler : MonoBehaviour, IConstructable
    {
        public BaseModuleProvider moduleProvider;

        public bool CanDeconstruct(out string reason)
        {
            if (moduleProvider == null)
            {
                reason = default;
                return true;
            }

            return moduleProvider.CanDeconstruct(out reason);
        }

        public bool IsDeconstructionObstacle()
        {
            return true;
        }

        public void OnConstructedChanged(bool constructed)
        {
            
        }
    }
}
