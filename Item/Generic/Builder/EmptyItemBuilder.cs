using Nautilus.Assets;
using Nautilus.Utility;
using System;
using UnityEngine;

namespace Industrica.Item.Generic.Builder
{
    public class EmptyItemBuilder : AbstractItemBuilder
    {
        public EmptyItemBuilder(string classID) : base(classID) { }

        public override void Build(out PrefabInfo info)
        {
            GetPrefab(out info, out CustomPrefab prefab);
            prefab.SetGameObject(GetGameObject(info));
            prefab.Register();
        }

        private Func<GameObject> GetGameObject(in PrefabInfo info)
        {
            GameObject prefab = new();
            PrefabUtils.AddBasicComponents(prefab, info.ClassID, info.TechType, LargeWorldEntity.CellLevel.Near);
            prefab.EnsureComponent<Pickupable>();
            return () => prefab;
        }
    }
}
