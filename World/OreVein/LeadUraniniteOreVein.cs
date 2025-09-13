using Industrica.Item.Generic;
using Nautilus.Assets;
using UnityEngine;

namespace Industrica.World.OreVein
{
    public class LeadUraniniteOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinLeadUraninite", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<LeadUraniniteOreVein>(Info, VeinRange));
            prefab.Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.Mountains_CaveFloor, new BiomeOreValidator(Vein, BiomeType.Mountains_CaveFloor, 2, 0.2f) }
            }, MinDepth: 330f).Register();

            Bounds caveBounds = new(center: new Vector3(-847f, -443f, 1265f), size: new Vector3(160f, 40f, 140f));
            new OreVeinBoundedSpawner(VeinGuaranteed, Info.ClassID, VeinRange, new()
            {
                { BiomeType.BloodKelp_CaveFloor, new BiomeOreValidator(VeinGuaranteed, BiomeType.BloodKelp_CaveFloor, 1, 1f) }
            }, Bounds: caveBounds).Register();

            new OreVeinBoundedSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.BloodKelp_CaveFloor, new BiomeOreValidator(Vein, BiomeType.BloodKelp_CaveFloor, 2, 0.1f) }
            }, Bounds: caveBounds).Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.BloodKelp_Floor, new BiomeOreValidator(Vein, BiomeType.BloodKelp_Floor, 2, 0.05f) }
            }, MinDepth: 500f).Register();
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceLeadUraninite.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleLeadUraninite.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 3f;
        public const OreVeinType Vein = OreVeinType.LeadUraninite;
        public const OreVeinType VeinGuaranteed = OreVeinType.LeadUraniniteGuaranteed;
    }
}
