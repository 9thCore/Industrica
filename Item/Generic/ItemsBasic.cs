using Industrica.Item.Generic.Builder;
using Industrica.Utility;
using Nautilus.Assets;

namespace Industrica.Item.Generic
{
    public static class ItemsBasic
    {
        public static PrefabInfo CoreSampleEmpty;
        public static PrefabInfo CoreSampleTitaniumCopper;
        public static PrefabInfo CoreSampleCopperSilver;
        public static PrefabInfo CoreSampleQuartzDiamond;
        public static PrefabInfo CoreSampleSilverGold;
        public static PrefabInfo CoreSampleLeadUraninite;
        public static PrefabInfo CoreSampleMagnetiteLithium;
        public static PrefabInfo CoreSampleRubyKyanite;

        public static PrefabInfo OreVeinResourceEmpty;
        public static PrefabInfo OreVeinResourceTitaniumCopper;
        public static PrefabInfo OreVeinResourceCopperSilver;
        public static PrefabInfo OreVeinResourceQuartzDiamond;
        public static PrefabInfo OreVeinResourceSilverGold;
        public static PrefabInfo OreVeinResourceLeadUraninite;
        public static PrefabInfo OreVeinResourceMagnetiteLithium;
        public static PrefabInfo OreVeinResourceRubyKyanite;

        public static PrefabInfo Slag;

        public static void Register()
        {
            new CloneItemBuilder($"Industrica{nameof(CoreSampleEmpty)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleEmpty);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleTitaniumCopper)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleTitaniumCopper);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleCopperSilver)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleCopperSilver);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleQuartzDiamond)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleQuartzDiamond);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleSilverGold)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleSilverGold);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleLeadUraninite)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleLeadUraninite);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleMagnetiteLithium)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleMagnetiteLithium);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleRubyKyanite)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleRubyKyanite);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceEmpty)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceEmpty);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceTitaniumCopper)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceTitaniumCopper);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceCopperSilver)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceCopperSilver);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceQuartzDiamond)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceQuartzDiamond);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceSilverGold)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceSilverGold);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceLeadUraninite)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceLeadUraninite);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceMagnetiteLithium)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceMagnetiteLithium);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceRubyKyanite)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceRubyKyanite);

            new CloneItemBuilder($"Industrica{nameof(Slag)}", TechType.SeaTreaderPoop, LargeWorldEntity.CellLevel.Near)
                .Build(out Slag);
        }
    }
}
