using Industrica.Fluid.Generic.Attributes;
using Industrica.Fluid.Generic.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Industrica.Fluid.Generic
{
    public static class FluidsBasic
    {
        [Fluid(0f, 0f, 1f)]
        public static FluidInfo Seawater;

        private static IEnumerable<FieldInfo> ItemDefinitions => typeof(FluidsBasic)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(info => info.FieldType == typeof(FluidInfo));

        public static void Register()
        {
            RegisterAuto();
        }

        public static void RegisterAuto()
        {
            foreach (FieldInfo field in ItemDefinitions)
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(field);

                FluidBuilder fluidBuilder = null;

                foreach (Attribute attribute in attributes)
                {
                    if (attribute is FluidAttribute fluidAttribute)
                    {
                        fluidBuilder = fluidAttribute.GetBuilder($"IndustricaFluid{field.Name}");
                        break;
                    }
                }

                if (fluidBuilder == null)
                {
                    continue;
                }                

                fluidBuilder.Build(out FluidInfo prefabInfo);
                field.SetValue(null, prefabInfo);
            }
        }
    }
}
