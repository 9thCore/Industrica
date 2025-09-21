using Industrica.Item.Generic;
using Nautilus.Assets;

namespace Industrica.World.OreVein
{
    public class LithiumNickelOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinLithiumNickel", false)
                .WithIcon(OreVeinSprite);

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<LithiumNickelOreVein>(Info, VeinRange));
            prefab.Register();

            float minimumDepth = 820f;

            new OreVeinDepthSpawner(VeinGuaranteed, Info.ClassID, VeinRange, new()
            {
                { BiomeType.LostRiverJunction_Ground, new BiomeOreValidator(VeinGuaranteed, BiomeType.LostRiverJunction_Ground, 1, 1f) }
            }, MinDepth: minimumDepth).Register();

            new OreVeinDepthSpawner(Vein, Info.ClassID, VeinRange, new()
            {
                { BiomeType.LostRiverJunction_Ground, new BiomeOreValidator(Vein, BiomeType.LostRiverJunction_Ground, 3, 0.05f) }
            }, MinDepth: minimumDepth).Register();
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceLithiumNickel.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleLithiumNickel.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 9f;
        public const OreVeinType Vein = OreVeinType.LithiumNickel;
        public const OreVeinType VeinGuaranteed = OreVeinType.LithiumNickelGuaranteed;
    }
}
