using Industrica.Item.Generic.Attributes;
using Industrica.Item.Generic.Builder;
using Nautilus.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Industrica.Item.Generic
{
    public static class ItemsBasic
    {
        [CloneItem(TechType.LabContainer3, LargeWorldEntity.CellLevel.Near)]
        public static PrefabInfo CoreSampleEmpty,
            CoreSampleTitaniumCopper,
            CoreSampleCopperSilver,
            CoreSampleQuartzDiamond,
            CoreSampleSilverGold,
            CoreSampleLeadUraninite,
            CoreSampleMagnetiteLithium,
            CoreSampleRubyKyanite;

        [CloneItem(TechType.LimestoneChunk, LargeWorldEntity.CellLevel.Near)]
        [ItemBreakableChunkPrefabModifier]
        public static PrefabInfo OreVeinResourceEmpty,
            OreVeinResourceTitaniumCopper,
            OreVeinResourceCopperSilver,
            OreVeinResourceQuartzDiamond,
            OreVeinResourceSilverGold,
            OreVeinResourceLeadUraninite,
            OreVeinResourceMagnetiteLithium,
            OreVeinResourceRubyKyanite;

        [CloneItem(TechType.SeaTreaderPoop, LargeWorldEntity.CellLevel.Near)]
        public static PrefabInfo Slag;

        private static IEnumerable<FieldInfo> ItemDefinitions => typeof(ItemsBasic)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(info => info.FieldType == typeof(PrefabInfo));

        public static void Register()
        {
            RegisterAuto();
        }

        public static void RegisterAuto()
        {
            foreach (FieldInfo field in ItemDefinitions)
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(field);

                AbstractItemBuilder itemBuilder = null;

                // Ensure the builder is set up before applying modifiers
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is AbstractItemAttribute itemAttribute)
                    {
                        itemBuilder = itemAttribute.GetBuilder($"Industrica{field.Name}");
                        break;
                    }
                }

                if (itemBuilder == null)
                {
                    continue;
                }

                if (itemBuilder is CloneItemBuilder cloneItemBuilder)
                {
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is AbstractPrefabModifierAttribute prefabModifierAttribute)
                        {
                            prefabModifierAttribute.Apply(cloneItemBuilder);
                        }
                    }
                }

                itemBuilder.Build(out PrefabInfo prefabInfo);
                field.SetValue(null, prefabInfo);
            }
        }
    }
}
