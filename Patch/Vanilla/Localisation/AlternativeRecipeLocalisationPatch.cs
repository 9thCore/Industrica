using HarmonyLib;
using Industrica.Utility;

namespace Industrica.Patch.Vanilla.Localisation
{
    [HarmonyPatch(typeof(Language), nameof(Language.LoadLanguageFile))]
    public static class AlternativeRecipeLocalisationPatch
    {
        public static void Postfix(Language __instance, bool __result)
        {
            if (!__result)
            {
                return;
            }

            RecipeUtil.TranslationRemapping.ForEach(pair =>
            {
                string cloneName = pair.Key.AsString();
                string cloneTooltip = $"Tooltip_{cloneName}";

                string resultName = pair.Value.Result.AsString();
                string resultTooltip = $"Tooltip_{resultName}";

                if (pair.Value.Count > 1)
                {
                    __instance.strings[cloneName] = $"{resultName.Translate()} (x{pair.Value.Count})";
                } else
                {
                    __instance.strings[cloneName] = pair.Value.Result.AsString().Translate();
                }

                __instance.strings[cloneTooltip] = resultTooltip.Translate();
            });
        }
    }
}
