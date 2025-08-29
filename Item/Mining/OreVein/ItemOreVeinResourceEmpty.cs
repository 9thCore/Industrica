using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System;
using UnityEngine;

namespace Industrica.Item.Mining.OreVein
{
    public static class ItemOreVeinResourceEmpty
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinResource_Empty")
                .WithIcon(SpriteManager.Get(TechType.LimestoneChunk));

            var prefab = new CustomPrefab(Info);
            var template = new CloneTemplate(Info, TechType.LimestoneChunk);

            template.ModifyPrefab += ModifyBreakableChunkPrefab(Info);

            prefab.SetGameObject(template);
            prefab.Register();
        }

        public static void ModifyBreakableChunkPrefab(GameObject prefab, PrefabInfo info)
        {
            PrefabUtils.AddBasicComponents(prefab, info.ClassID, info.TechType, LargeWorldEntity.CellLevel.Near);

            Rigidbody rigidbody = prefab.EnsureComponent<Rigidbody>();
            rigidbody.isKinematic = false;

            ResourceTracker tracker = prefab.EnsureComponent<ResourceTracker>();
            tracker.rb = rigidbody;
            tracker.pickupable = prefab.EnsureComponent<Pickupable>();

            GameObject.DestroyImmediate(prefab.GetComponent<VFXBurstModel>());
            GameObject.DestroyImmediate(prefab.GetComponent<BreakableResource>());
            GameObject.DestroyImmediate(prefab.GetComponent<EcoTarget>());
        }

        public static Action<GameObject> ModifyBreakableChunkPrefab(PrefabInfo info)
        {
            return prefab => ModifyBreakableChunkPrefab(prefab, info);
        }
    }
}
