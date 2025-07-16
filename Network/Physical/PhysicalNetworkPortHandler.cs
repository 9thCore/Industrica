using System.Linq;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public class PhysicalNetworkPortHandler<T> : MonoBehaviour, IConstructable
    {
        private PhysicalNetworkPort<T>[] ports = null;
        private int count = 0;

        public string GetClassID()
        {
            return $"PhysicalNetworkPort{count++}";
        }

        public void Start()
        {
            ports = GetComponentsInChildren<PhysicalNetworkPort<T>>(true);
        }

        public bool CanDeconstruct(out string reason)
        {
            if (ports == null || ports.All(c => c == null || !c.Occupied))
            {
                reason = default;
                return true;
            }

            reason = Language.main.Get("IndustricaPort_CannotDeconstructConnected");
            return false;
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
