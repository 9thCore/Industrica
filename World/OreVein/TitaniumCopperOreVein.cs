using Industrica.Item.Generic;
using Nautilus.Assets;

namespace Industrica.World.OreVein
{
    public class TitaniumCopperOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinTitaniumCopper", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<TitaniumCopperOreVein>(Info, VeinRange));
            prefab.Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.SparseReef_Sand, new BiomeOreValidator(Vein, BiomeType.SparseReef_Sand, 2, 0.1f) },
                { BiomeType.SparseReef_DeepFloor, new BiomeOreValidator(Vein, BiomeType.SparseReef_DeepFloor, 3, 0.1f) }
            }, MinDepth: 200f).Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.Mountains_Sand, new BiomeOreValidator(Vein, BiomeType.Mountains_Sand, 2, 0.09f) },
                { BiomeType.KooshZone_Sand, new BiomeOreValidator(Vein, BiomeType.KooshZone_Sand, 2, 0.015f) }
            }, MinDepth: 200f, MaxDepth: 340f).Register();
        }
        
        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceTitaniumCopper.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleTitaniumCopper.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 5f;
        public const OreVeinType Vein = OreVeinType.TitaniumCopper;
    }
}
