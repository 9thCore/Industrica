using UnityEngine;

namespace Industrica.Network
{
    public class PortIDAssigner : MonoBehaviour
    {
        private int count = 0;

        public string GetClassIDAndCycle()
        {
            return $"PortIdentifier_{count++}";
        }
    }
}
