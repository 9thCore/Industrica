using Industrica.Item.Generic.Builder;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Crafting;
using System.Collections.Generic;

namespace Industrica.Recipe.Handler
{
    public static class GeneralFakeIngredients
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
            return catalystInfo.TechType;
        }
        
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
