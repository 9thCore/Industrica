using Nautilus.Handlers;
using System.Reflection;
using UnityEngine;

namespace Industrica.Fluid
{
    public record struct FluidInfo(string ClassID, TechType TechType)
    {
        public static FluidInfo WithTechType(string classID)
        {
            Assembly owner = Assembly.GetCallingAssembly();

            return new FluidInfo(classID, EnumHandler.AddEntry<TechType>(classID, owner));
        }

        public readonly FluidInfo WithIcon(Sprite sprite)
        {
            SpriteHandler.RegisterSprite(TechType, sprite);
            return this;
        }
    }
}
