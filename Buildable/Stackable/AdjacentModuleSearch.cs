using UnityEngine;

namespace Industrica.Buildable.Stackable
{
    public class AdjacentModuleSearch<T> where T : class, IBaseModuleGeometry
    {
        public Base.Direction Direction { get; private set; }
        public T Module { get; private set; }

        private double nextCheck = 0d;
        private MonoBehaviour component;
        private Base seabase;

        public delegate void FoundModuleHandler(T module);
        public event FoundModuleHandler FoundModule;

        public AdjacentModuleSearch(Base.Direction direction)
        {
            Direction = direction;
        }

        public void Link(MonoBehaviour component)
        {
            this.component = component;

            GameObject gameObject = component.gameObject;

            seabase = gameObject.GetComponentInParent<Base>();
            if (seabase == null)
            {
                Plugin.Logger.LogError($"Could not find {nameof(Base)} component in the tree for {gameObject}. Disabling component");
                component.enabled = false;
            }
        }

        public void CheckForModule(bool force = false)
        {
            // No need to bother checking if we still have a reference
            // Interfaces stink so .Equals is required here
            bool referenceRemoved = Module == null || Module.Equals(null);

            if (!referenceRemoved
                || (!force && DayNightCycle.main.timePassed < nextCheck))
            {
                return;
            }
            nextCheck = DayNightCycle.main.timePassed + 1d;

            Int3 cell = seabase.WorldToGrid(component.transform.position);
            Grid3Point point = seabase.baseShape.GetPoint(cell);

            int index = Direction switch
            {
                Base.Direction.Above => seabase.baseShape.GetAboveIndex(ref point),
                Base.Direction.Below => seabase.baseShape.GetBelowIndex(ref point),
                _ => 0
            };

            Int3 searched = seabase.baseShape.GetPointAsInt3(index);
            Base.Face face = new Base.Face(searched, Base.Direction.Below);

            if (seabase.GetModuleGeometry(face) is not T geometry)
            {
                return;
            }

            Module = geometry;
            FoundModule?.Invoke(geometry);
        }

        public void Invalidate()
        {
            Module = null;
        }
    }
}
