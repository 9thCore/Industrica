using Industrica.Item.Generic;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.World.OreVein
{
    public class TitaniumCopperOreVein : AbstractOreVein
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinTitaniumCopper", false)
                .WithIcon(SpriteManager.Get(TechType.LimestoneChunk));

            var prefab = new CustomPrefab(Info);

            prefab.SetGameObject(() => GetGameObject<TitaniumCopperOreVein>(Info, VeinRange));
            prefab.Register();

            Setup(Info, VeinRange, new Dictionary<BiomeType, WorldUtil.BiomeValidator>()
            {
                { BiomeType.GrassyPlateaus_Grass, new BiomeOreValidator(BiomeType.GrassyPlateaus_Grass, 3, 0.004f) }
            });
        }

        public override TechType ResourceTechType => ItemsBasic.OreVeinResourceTitaniumCopper.TechType;
        public override TechType OreVeinTechType => Info.TechType;
        public override TechType CoreSampleTechType => ItemsBasic.CoreSampleTitaniumCopper.TechType;
        public override float Range => VeinRange;

        public const float VeinRange = 5f;
    }
}
