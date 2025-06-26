using UnityEngine;

namespace Industrica.ClassBase
{
    public abstract class BaseConstructable : MonoBehaviour
    {
        protected Constructable constructable;

        public bool Constructed => constructable != null && constructable.constructed;
        public float ConstructedAmount => constructable == null ? 0f : constructable.constructedAmount;

        public virtual void Start()
        {
            if (!TryGetComponent(out constructable))
            {
                Plugin.Logger.LogError($"Could not find {nameof(Constructable)} in {gameObject.name}. Disabling...");
                enabled = false;
            }
        }
    }
}
