using Industrica.Item.Generic.Builder;
using Industrica.Utility;
using Nautilus.Assets;
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
            CatalystToOriginal.Add(catalystInfo.TechType, techType);
            return catalystInfo.TechType;
        }

        public static TechType GetOrCreateTimeIngredientFor(float craftTime)
        {
            string formatted = FormatCraftTime(craftTime);

            if (CachedCraftTimes.TryGetValue(formatted, out TechType ingredient))
            {
                return ingredient;
            }

            new CloneItemBuilder($"IndustricaCraftTime{formatted}", TechType.MapRoomUpgradeScanSpeed, LargeWorldEntity.CellLevel.Near)
                    .Build(out PrefabInfo timeInfo);

            LocalizationUtil.RegisterLocalizationData(new RuntimeCraftTimeLocalizationData(timeInfo.TechType, craftTime));

            RegisterFakeIngredient(timeInfo.TechType);
            CachedCraftTimes.Add(formatted, timeInfo.TechType);
            return timeInfo.TechType;
        }

        private static string FormatCraftTime(float craftTime)
        {
            return craftTime.ToString("0.##");
        }

        public static bool IsIngredientFake(TechType techType)
        {
            return FakeIngredients.Contains(techType);
        }

        public static void RegisterFakeIngredient(TechType techType)
        {
            FakeIngredients.Add(techType);
        }

        public static bool TryGetOriginalFromCatalyst(TechType catalyst, out TechType original)
        {
            return CatalystToOriginal.TryGetValue(catalyst, out original);
        }

        public static void Clear()
        {
            CachedCatalysts.Clear();
            CachedCraftTimes.Clear();
        }

        private static readonly HashSet<TechType> FakeIngredients = new();
        private static readonly Dictionary<TechType, TechType> CatalystToOriginal = new();
        private static readonly Dictionary<TechType, TechType> CachedCatalysts = new();
        private static readonly Dictionary<string, TechType> CachedCraftTimes = new();

        private record RuntimeCatalystLocalizationData(TechType Catalyst, TechType Original)
            : LocalizationUtil.RuntimeTechTypeLocalizationData(Catalyst)
        {
            public override string GetTranslation()
            {
                return LocalizationUtil.CatalystKey.Translate(Original.AsString().Translate());
            }
        }

        private record RuntimeCraftTimeLocalizationData(TechType Time, float CraftTime)
            : LocalizationUtil.RuntimeTechTypeLocalizationData(Time)
        {
            public override string GetTranslation()
            {
                return LocalizationUtil.TimeKey.Translate(FormatCraftTime(CraftTime));
            }
        }
    }
}
