using Industrica.Network.Physical;
using System.Linq;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class PortHandler : MonoBehaviour, IConstructable
    {
        private int count = 0;

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
    }
}
