using Industrica.Buildable.Stackable;
using Industrica.Patch.Buildable.AlienContainmentUnit;
using System;
using UnityEngine;

namespace Industrica.Patch.Buildable.NuclearReactor
{
    public class NuclearReactorSteamer : MonoBehaviour
    {
        private readonly AdjacentModuleSearch<WaterParkGeometry> lookup = new(Base.Direction.Above);
        private BaseNuclearReactor reactor;

        public void Start()
        {
            lookup.Link(this);
            lookup.FoundModule += FoundModule;
            
            if (!TryGetComponent(out reactor))
            {
                Plugin.Logger.LogError($"Could not find {nameof(BaseNuclearReactor)} component in {gameObject}. Disabling component");
                enabled = false;
                throw new InvalidOperationException();
            }
        }

        public void Update()
        {
            lookup.CheckForModule();
        }

        public void FoundModule(WaterParkGeometry module)
        {
            WaterParkSteamer steamer = WaterParkSteamer.Attach(module);
            steamer.SetReactor(reactor, true);
        }
    }
}
