using Industrica.Item.Generic;
using Nautilus.Assets;

namespace Industrica.World.OreVein
{
    public class CopperSilverOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinCopperSilver", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<CopperSilverOreVein>(Info, VeinRange));
            prefab.Register();

            new OreVeinSpawner(Info.ClassID, VeinRange, new()
            {
                { BiomeType.JellyshroomCaves_CaveFloor, new BiomeOreValidator(BiomeType.JellyshroomCaves_CaveFloor, 2, 0.25f) }
            }).Register();

            new OreVeinDepthSpawner(Info.ClassID, VeinRange, new()
            {
                { BiomeType.CragField_Ground, new BiomeOreValidator(BiomeType.CragField_Ground, 5, 0.1f) },
            }, MinDepth: 175f).Register();

            new OreVeinDepthSpawner(Info.ClassID, VeinRange, new()
            {
                { BiomeType.MushroomForest_Grass, new BiomeOreValidator(BiomeType.MushroomForest_Grass, 2, 0.2f) }
            }, MinDepth: 200f).Register();
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceCopperSilver.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleCopperSilver.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 5f;
    }
}
