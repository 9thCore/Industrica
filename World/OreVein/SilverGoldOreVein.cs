using Industrica.Item.Generic;
using Nautilus.Assets;
using UnityEngine;

namespace Industrica.World.OreVein
{
    public class SilverGoldOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinSilverGold", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<SilverGoldOreVein>(Info, VeinRange));
            prefab.Register();

            float dunesDepth = 345f;
            new OreVeinDepthSpawner(VeinGuaranteed, Info.ClassID, VeinRange, new()
            {
                { BiomeType.Dunes_SandDune, new BiomeOreValidator(VeinGuaranteed, BiomeType.Dunes_SandDune, 3, 1f) }
            }, MinDepth: dunesDepth).Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.Dunes_SandDune, new BiomeOreValidator(Vein, BiomeType.Dunes_SandDune, 2, 0.08f) }
            }, MinDepth: dunesDepth).Register();

            new OreVeinSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.CrashZone_TrenchSand, new BiomeOreValidator(Vein, BiomeType.CrashZone_TrenchSand, 2, 0.01f) }
            }).Register();

            Vector3 genericSize60 = Vector3.one * 60f;
            Vector3 genericSize65 = Vector3.one * 65f;
            Bounds[] validIslands =
            {
                new(center: new Vector3(-115f, -273f, 1002f), size: new Vector3(218f, 74f, 218f)),
                new(center: new Vector3(16f, -192f, 912f), size: genericSize60),
                new(center: new Vector3(16f, -207f, 1119f), size: genericSize60),
                new(center: new Vector3(-144f, -238f, 1151f), size: genericSize65),
                new(center: new Vector3(-303f, -223f, 1070f), size: genericSize65),
                new(center: new Vector3(-270f, -191f, 960f), size: genericSize60)
            };
            foreach (Bounds island in validIslands)
            {
                new OreVeinBoundedSpawner(Vein, Info.ClassID, VeinRange, new()
                {
                    { BiomeType.UnderwaterIslands_IslandTop, new BiomeOreValidator(Vein, BiomeType.UnderwaterIslands_IslandTop, 2, 0.08f) }
                }, Bounds: island).Register();
            }
        }
        
        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceSilverGold.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleSilverGold.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 9f;
        public const OreVeinType Vein = OreVeinType.SilverGold;
        public const OreVeinType VeinGuaranteed = OreVeinType.SilverGoldGuaranteed;
    }
}
