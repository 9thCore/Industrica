using Industrica.Utility;
using Nautilus.Assets;
using UnityEngine;

namespace Industrica.Fluid.Generic.Builder
{
    public class FluidBuilder
    {
        protected readonly string classID;
        protected Sprite icon;

        public FluidBuilder(string classID)
        {
            this.classID = classID;
        }

        public FluidBuilder WithIcon(Sprite icon)
        {
            this.icon = icon;
            return this;
        }

        public virtual void Build(out FluidInfo fluidInfo)
        {
            fluidInfo = GetFluidInfo();
            FluidUtil.fluids.Add(fluidInfo.TechType);
        }

        protected FluidInfo GetFluidInfo()
        {
            FluidInfo info = FluidInfo.WithTechType(classID);

            if (icon != null)
            {
                info = info.WithIcon(icon);
            } else
            {
                info = info.WithIcon(SpriteManager.Get(TechType.Lubricant));
            }

            return info;
        }

        protected PrefabInfo GetItemRepresentation()
        {
            PrefabInfo info = PrefabInfo.WithTechType(classID);

            if (icon != null)
            {
                info = info.WithIcon(icon);
            } else
            {
                info = info.WithIcon(SpriteManager.Get(TechType.Lubricant));
            }

            return info;
        }
    }
}
