namespace Industrica.Utility
{
    public static class StringUtil
    {
        public static string AsLoadTaskID(this string suffix)
        {
            return $"{nameof(Industrica)}.{suffix}";
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
    }
}
