using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Item.Generic.Builder
{
    public class CloneItemBuilder : TemplatedItemBuilder
    {
        protected readonly LargeWorldEntity.CellLevel cellLevel;
        protected readonly TechType cloneTechType;
        protected readonly string cloneClassID;
        protected readonly List<Action<GameObject>> modifyPrefab = new();

        public CloneItemBuilder(string classID, TechType cloneTechType, LargeWorldEntity.CellLevel cellLevel) : base(classID)
        {
            this.cloneTechType = cloneTechType;
            this.cellLevel = cellLevel;
            WithIcon(SpriteManager.Get(cloneTechType));
        }

        public CloneItemBuilder(string classID, string cloneClassID, LargeWorldEntity.CellLevel cellLevel) : base(classID)
        {
            this.cloneClassID = cloneClassID;
            this.cellLevel = cellLevel;
        }

        public CloneItemBuilder ModifyPrefab(Action<GameObject> callback)
        {
            modifyPrefab.Add(callback);
            return this;
        }

        protected override PrefabTemplate CreateTemplate(in PrefabInfo info)
        {
            CloneTemplate template = GetCloneTemplate(info);
            string classID = info.ClassID;
            TechType techType = info.TechType;
            template.ModifyPrefab += prefab => PrefabUtils.AddBasicComponents(prefab, classID, techType, cellLevel);
            modifyPrefab.ForEach(callback => template.ModifyPrefab += callback);
            return template;
        }

        private CloneTemplate GetCloneTemplate(in PrefabInfo info)
        {
            if (string.IsNullOrEmpty(cloneClassID))
            {
                return new CloneTemplate(info, cloneTechType);
            }

            return new CloneTemplate(info, cloneClassID);
        }
    }
}
