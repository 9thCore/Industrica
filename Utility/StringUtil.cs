namespace Industrica.Utility
{
    public static class StringUtil
    {
        public static string AsLoadTaskID(this string suffix)
        {
            return $"{nameof(Industrica)}.{suffix}";
        }
    }
}
