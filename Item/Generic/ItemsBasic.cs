using Industrica.Item.Generic.Builder;
using Industrica.Utility;
using Nautilus.Assets;

namespace Industrica.Item.Generic
{
    public static class ItemsBasic
    {
        public static PrefabInfo CoreSampleEmpty;
        public static PrefabInfo CoreSampleTitaniumCopper;

        public static PrefabInfo OreVeinResourceEmpty;
        public static PrefabInfo OreVeinResourceTitaniumCopper;

        public static PrefabInfo Slag;

        public static void Register()
        {
            new CloneItemBuilder($"Industrica{nameof(CoreSampleEmpty)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleEmpty);

            new CloneItemBuilder($"Industrica{nameof(CoreSampleTitaniumCopper)}", TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)
                .Build(out CoreSampleTitaniumCopper);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceEmpty)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceEmpty);

            new CloneItemBuilder($"Industrica{nameof(OreVeinResourceTitaniumCopper)}", TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)
                .ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem)
                .Build(out OreVeinResourceTitaniumCopper);

            new CloneItemBuilder($"Industrica{nameof(Slag)}", TechType.SeaTreaderPoop, LargeWorldEntity.CellLevel.Near)
                .Build(out Slag);
        }
    }
}
