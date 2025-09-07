using HarmonyLib;
using Industrica.Recipe;

namespace Industrica.Patch.Vanilla.Container
{
    [HarmonyPatch(typeof(ItemsContainer), nameof(ItemsContainer.GetCount))]
    public static class ItemsContainerCountPatch
    {
        public static void Postfix(ItemsContainer __instance, TechType techType, ref int __result)
        {
            if (ExtraIngredientHelper.TryGetOriginalFromCatalyst(techType, out TechType original))
            {
                __result += __instance.GetCount(original);
            }
        }
    }
}
