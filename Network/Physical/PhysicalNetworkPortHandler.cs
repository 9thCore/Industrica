using System.Collections;
using System.Linq;
using UnityEngine;
using UWE;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPortHandler<T> : MonoBehaviour, IConstructable where T : class
    {
        private int count = 0;

        public abstract void Fetch();
        public abstract bool CanDeconstructPorts();

        public IEnumerator QueueFetch()
        {
            yield return null;
            Fetch();
            yield break;
        }

        public string GetClassID()
        {
            return $"PhysicalNetworkPort{count++}";
        }

        public bool CanDeconstruct(out string reason)
        {
            if (!CanDeconstructPorts())
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
