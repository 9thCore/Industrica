using Industrica.Item.Mining.CoreSample;
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
                .WithTechType("IndustricaOreVein_TitaniumCopper", false)
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
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemCoreSampleTitaniumCopper.Info.TechType;
    }
}
