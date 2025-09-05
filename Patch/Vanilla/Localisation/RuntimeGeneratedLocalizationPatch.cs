using HarmonyLib;
using Industrica.Utility;

namespace Industrica.Patch.Vanilla.Localisation
{
    [HarmonyPatch(typeof(Language), nameof(Language.LoadLanguageFile))]
    public static class RuntimeGeneratedLocalizationPatch
    {
        public static void Postfix(Language __instance, bool __result)
        {
            if (!__result)
            {
                return;
            }

            LocalizationUtil.ApplyLocalizationData(__instance);
        }
    }
}
