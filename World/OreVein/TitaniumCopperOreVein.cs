using Industrica.Item.Generic;
using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.World.OreVein
{
    public class TitaniumCopperOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinTitaniumCopper", false)
                .WithIcon(SpriteManager.Get(TechType.LimestoneChunk));

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(GetGameObject);
            prefab.Register();
        }

        private static GameObject GetGameObject()
        {
            GameObject prefab = new GameObject();

            PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);

            Setup(prefab);
            prefab.EnsureComponent<TitaniumCopperOreVein>();

            return prefab;
        }

        public override float Range => 10f;
        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceTitaniumCopper.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleTitaniumCopper.TechType;
    }
}
