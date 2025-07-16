using System.Linq;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public class PhysicalNetworkPortHandler<T> : MonoBehaviour, IConstructable
    {
        public PhysicalNetworkPort<T>[] Ports { get; private set; }
        private int count = 0;

        public string GetClassID()
        {
            return $"PhysicalNetworkPort{count++}";
        }

        public void Start()
        {
            Ports = GetComponentsInChildren<PhysicalNetworkPort<T>>(true);
        }

        public bool CanDeconstruct(out string reason)
        {
            if (Ports == null || Ports.All(c => c == null || !c.Occupied))
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
