using Industrica.Fluid.Generic.Builder;
using System;

namespace Industrica.Fluid.Generic.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FluidAttribute : Attribute
    {
        public readonly float r, g, b;

        public FluidAttribute(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public FluidBuilder GetBuilder(string classID)
        {
            return new FluidBuilder(classID);
        }
    }
}
