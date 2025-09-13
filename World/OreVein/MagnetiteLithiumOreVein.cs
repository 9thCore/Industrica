using Industrica.Item.Generic;
using Nautilus.Assets;

namespace Industrica.World.OreVein
{
    public class MagnetiteLithiumOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinMagnetiteLithium", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<MagnetiteLithiumOreVein>(Info, VeinRange));
            prefab.Register();

            new OreVeinSpawner(VeinGuaranteed, Info.ClassID, VeinRange, new()
            {
                { BiomeType.JellyshroomCaves_CaveSand, new BiomeOreValidator(VeinGuaranteed, BiomeType.JellyshroomCaves_CaveSand, 1, 1f) }
            }).Register();

            new OreVeinSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.JellyshroomCaves_CaveSand, new BiomeOreValidator(Vein, BiomeType.JellyshroomCaves_CaveSand, 3, 0.5f) }
            }).Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.BloodKelp_Floor, new BiomeOreValidator(Vein, BiomeType.BloodKelp_Floor, 2, 0.05f) }
            }, MaxDepth: 300f).Register();

            new OreVeinSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.BonesField_Ground, new BiomeOreValidator(Vein, BiomeType.BonesField_Ground, 2, 0.05f) }
            }).Register();
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceMagnetiteLithium.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleMagnetiteLithium.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 4f;
        public const OreVeinType Vein = OreVeinType.MagnetiteLithium;
        public const OreVeinType VeinGuaranteed = OreVeinType.MagnetiteLithiumGuaranteed;
    }
}
