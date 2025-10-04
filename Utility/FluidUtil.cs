using System.Collections.Generic;

namespace Industrica.Utility
{
    public static class FluidUtil
    {
        public static bool IsFluid(this TechType techType)
        {
            return fluids.Contains(techType);
        }

        public static readonly HashSet<TechType> fluids = new();
    }
}
