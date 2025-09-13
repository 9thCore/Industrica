using Industrica.Item.Generic;
using Nautilus.Assets;
using UnityEngine;

namespace Industrica.World.OreVein
{
    internal class QuartzDiamondOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinQuartzDiamond", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<QuartzDiamondOreVein>(Info, VeinRange));
            prefab.Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.DeepGrandReef_Ground, new BiomeOreValidator(Vein, BiomeType.DeepGrandReef_Ground, 2, 0.1f) }
            }, MinDepth: 500f).Register();

            Bounds kooshCaveSpawn = new Bounds(center: new Vector3(1225f, -263f, 528f), size: new Vector3(20f, 10f, 25f));
            new OreVeinBoundedSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.KooshZone_CaveFloor, new BiomeOreValidator(Vein, BiomeType.KooshZone_CaveFloor, 1, 0.75f) }
            }, Bounds: kooshCaveSpawn).Register();

            Bounds mountainsSpawn = new Bounds(center: new Vector3(660f, -380f, 1377f), size: new Vector3(130f, 20f, 200f));
            new OreVeinBoundedSpawner(VeinGuaranteed, Info.ClassID, VeinRange, new()
            {
                { BiomeType.Mountains_Sand, new BiomeOreValidator(VeinGuaranteed, BiomeType.Mountains_Sand, 1, 1f) }
            }, Bounds: mountainsSpawn).Register();

            new OreVeinBoundedSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.Mountains_Sand, new BiomeOreValidator(Vein, BiomeType.Mountains_Sand, 2, 0.5f) }
            }, Bounds: mountainsSpawn).Register();
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceQuartzDiamond.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleQuartzDiamond.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 4f;
        public const OreVeinType Vein = OreVeinType.QuartzDiamond;
        public const OreVeinType VeinGuaranteed = OreVeinType.QuartzDiamondGuaranteed;
    }
}
