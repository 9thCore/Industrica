using Industrica.Item.Generic;
using Nautilus.Assets;

namespace Industrica.World.OreVein
{
    public class RubyKyaniteOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinRubyKyanite", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<RubyKyaniteOreVein>(Info, VeinRange));
            prefab.Register();

            new OreVeinSpawner(VeinGuaranteed, Info.ClassID, VeinRange, new()
            {
                { BiomeType.ActiveLavaZone_Chamber_Floor, new BiomeOreValidator(VeinGuaranteed, BiomeType.ActiveLavaZone_Chamber_Floor, 1, 1f) }
            }).Register();

            new OreVeinSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.ActiveLavaZone_Falls_Floor, new BiomeOreValidator(Vein, BiomeType.ActiveLavaZone_Falls_Floor, 1, 0.3f) },
                { BiomeType.ActiveLavaZone_Chamber_Floor, new BiomeOreValidator(Vein, BiomeType.ActiveLavaZone_Chamber_Floor, 2, 0.2f) }
            }).Register();
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceRubyKyanite.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleRubyKyanite.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 7f;
        public const OreVeinType Vein = OreVeinType.RubyKyanite;
        public const OreVeinType VeinGuaranteed = OreVeinType.RubyKyaniteGuaranteed;
    }
}
