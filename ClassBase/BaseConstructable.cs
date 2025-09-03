using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.ClassBase
{
    public abstract class BaseConstructable : MonoBehaviour
    {
        protected Constructable constructable;

        public bool Constructed => constructable != null && constructable.constructed;
        public float ConstructedAmount => constructable == null ? 0f : constructable.constructedAmount;

        public virtual void Start()
        {
            constructable = GetComponentInParent<Constructable>();

            if (constructable == null)
            {
                Plugin.Logger.LogError($"Could not find {nameof(Constructable)} in {gameObject.name}'s hierarchy. Disabling...");
                enabled = false;
            }
        }
    }
}
