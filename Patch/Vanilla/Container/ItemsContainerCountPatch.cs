using HarmonyLib;
using Industrica.Recipe.Handler;

namespace Industrica.Patch.Vanilla.Container
{
    [HarmonyPatch(typeof(ItemsContainer), nameof(ItemsContainer.GetCount))]
    public static class ItemsContainerCountPatch
    {
        public static void Postfix(ItemsContainer __instance, TechType techType, ref int __result)
        {
            if (GeneralFakeIngredients.TryGetOriginalFromCatalyst(techType, out TechType original))
            {
                __result += __instance.GetCount(original);
            }
        }
    }
}
