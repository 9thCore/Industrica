using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public class PhysicalNetworkPortHandler : MonoBehaviour, IConstructable
    {
        private List<IPhysicalNetworkPort> ports;
        private int count = 0;

        public string GetClassID()
        {
            return $"PhysicalNetworkPort{count++}";
        }

        public void Start()
        {
            ports = GetComponentsInChildren<IPhysicalNetworkPort>().ToList();
        }

        public bool CanDeconstruct(out string reason)
        {
            if (ports.All(c => !c.Occupied))
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
