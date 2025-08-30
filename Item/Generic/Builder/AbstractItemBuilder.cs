using Nautilus.Assets;
using System;
using UnityEngine;

namespace Industrica.Item.Generic.Builder
{
    public abstract class AbstractItemBuilder
    {
        protected readonly string classID;
        protected Sprite icon;
        protected Vector2int? size = null;

        public AbstractItemBuilder(string classID)
        {
            this.classID = classID;
        }

        public AbstractItemBuilder WithIcon(Sprite icon)
        {
            this.icon = icon;
            return this;
        }

        public AbstractItemBuilder WithSizeInInventory(in Vector2int size)
        {
            this.size = size;
            return this;
        }

        protected void GetPrefab(out PrefabInfo info, out CustomPrefab prefab)
        {
            info = GetPrefabInfo();
            prefab = new(info);
        }

        public abstract void Build(out PrefabInfo info);

        private PrefabInfo GetPrefabInfo()
        {
            PrefabInfo info = PrefabInfo.WithTechType(classID);

            if (icon != null)
            {
                info = info.WithIcon(icon);
            }

            if (size.HasValue)
            {
                info = info.WithSizeInInventory(size.Value);
            }

            return info;
        }
    }
}
