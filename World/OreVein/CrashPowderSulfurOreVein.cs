using Industrica.Item.Generic;
using Nautilus.Assets;

namespace Industrica.World.OreVein
{
    public class CrashPowderSulfurOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinCrashPowderSulfur", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<CrashPowderSulfurOreVein>(Info, VeinRange));
            prefab.Register();

            float minDepth = 750f;
            new OreVeinDepthSpawner(VeinGuaranteed, Info.ClassID, VeinRange, new()
            {
                { BiomeType.BonesField_LakePit_Floor, new BiomeOreValidator(VeinGuaranteed, BiomeType.BonesField_LakePit_Floor, 1, 1f) }
            }, MinDepth: minDepth).Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.BonesField_LakePit_Floor, new BiomeOreValidator(Vein, BiomeType.BonesField_LakePit_Floor, 2, 0.3f) }
            }, MinDepth: minDepth).Register();
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceCrashPowderSulfur.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleCrashPowderSulfur.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 10f;
        public const OreVeinType Vein = OreVeinType.CrashPowderSulfur;
        public const OreVeinType VeinGuaranteed = OreVeinType.CrashPowderSulfurGuaranteed;
    }
}
