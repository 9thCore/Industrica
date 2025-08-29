using Industrica.Register.EcoTarget;
using UnityEngine;

namespace Industrica.World.OreVein
{
    public abstract class AbstractOreVein : MonoBehaviour
    {
        public float RangeSquared => Range * Range;
        public abstract float Range { get; }
        public abstract TechType ResourceTechType { get; }
        public abstract TechType OreVeinTechType { get; }
        public abstract TechType CoreSampleTechType { get; }

        protected static void Setup(GameObject prefab)
        {
            prefab.EnsureComponent<EcoTarget>().type = OreVeinEcoTarget.EcoTargetType;
            prefab.EnsureComponent<ResourceTracker>().prefabIdentifier = prefab.GetComponent<PrefabIdentifier>();
        }
    }
}
