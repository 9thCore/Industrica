using UnityEngine;

namespace Industrica.ClassBase
{
    public class DestroyableMonoBehaviour : MonoBehaviour, IDestroyable
    {
        public bool IsInstanceAlive => gameObject != null;
    }
}
