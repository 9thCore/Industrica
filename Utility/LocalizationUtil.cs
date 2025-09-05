using System.Collections.Generic;

namespace Industrica.Utility
{
    public static class LocalizationUtil
    {
        public const string AltRecipeOneResultKey = "IndustricaAlternativeRecipeOneResult";
        public const string AltRecipeMultipleResultsKey = "IndustricaAlternativeRecipeMultipleResults";
        public const string CatalystKey = "IndustricaCatalystFormat";

        public static string TranslateTooltip(this string localisationKey)
        {
            return Translate(localisationKey.GetTooltipKey());
        }

        public static string TranslateTooltip<T>(this string localisationKey, T value)
        {
            return Translate(localisationKey.GetTooltipKey(), value);
        }

        public static string TranslateTooltip<T1, T2>(this string localisationKey, T1 value1, T2 value2)
        {
            return Translate(localisationKey.GetTooltipKey(), value1, value2);
        }

        public static string Translate(this string localisationKey)
        {
            return Language.main.Get(localisationKey);
        }

        public static string Translate<T>(this string localisationKey, T value)
        {
            return Language.main.GetFormat(localisationKey, value);
        }

        public static string Translate<T1, T2>(this string localisationKey, T1 value1, T2 value2)
        {
            return Language.main.GetFormat(localisationKey, value1, value2);
        }

        public static string GetTooltipKey(this string localisationKey)
        {
            return $"Tooltip_{localisationKey}";
        }

        public static void RegisterLocalizationData(RuntimeLocalizationData data)
        {
            LocalizationData.Add(data);
        }

        public static void ApplyLocalizationData(Language language)
        {
            foreach (RuntimeLocalizationData data in LocalizationData)
            {
                language.strings[data.LocalizationKey] = data.GetTranslation();
            }
        }

        private static readonly List<RuntimeLocalizationData> LocalizationData = new();

        public abstract record RuntimeLocalizationData(string LocalizationKey)
        {
            public abstract string GetTranslation();
        }

        public abstract record RuntimeTechTypeLocalizationData(TechType TechType)
            : RuntimeLocalizationData(TechType.AsString());

        public abstract record RuntimeTechTypeTooltipLocalizationData(TechType TechType)
            : RuntimeLocalizationData(TechType.AsString().GetTooltipKey());
    }
}
