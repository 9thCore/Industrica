using Industrica.Item.Generic.Builder;
using Industrica.Utility;
using Nautilus.Assets;
using System.Collections.Generic;

namespace Industrica.Recipe
{
    public static class ExtraIngredientHelper
    {
        public static TechType GetOrCreateCatalystCloneFor(TechType techType)
        {
            if (CachedCatalysts.TryGetValue(techType, out TechType clone))
            {
                return clone;
            }

            new CloneItemBuilder($"IndustricaCatalyst{techType.AsString()}", techType, LargeWorldEntity.CellLevel.Near)
                    .Build(out PrefabInfo catalystInfo);

            LocalizationUtil.RegisterLocalizationData(new RuntimeCatalystLocalizationData(catalystInfo.TechType, techType));

            CachedCatalysts.Add(techType, catalystInfo.TechType);
            CatalystToOriginal.Add(catalystInfo.TechType, techType);
            return catalystInfo.TechType;
        }

        private static string FormatCraftTime(float craftTime)
        {
            return craftTime.ToString("0.##");
        }

        public static bool TryGetOriginalFromCatalyst(TechType catalyst, out TechType original)
        {
            return CatalystToOriginal.TryGetValue(catalyst, out original);
        }

        public static void Clear()
        {
            CachedCatalysts.Clear();
        }

        private static readonly Dictionary<TechType, TechType> CatalystToOriginal = new();
        private static readonly Dictionary<TechType, TechType> CachedCatalysts = new();

        private record RuntimeCatalystLocalizationData(TechType Catalyst, TechType Original)
            : LocalizationUtil.RuntimeTechTypeLocalizationData(Catalyst)
        {
            public override string GetTranslation()
            {
                return LocalizationUtil.CatalystKey.Translate(Original.AsString().Translate());
            }
        }
    }
}
