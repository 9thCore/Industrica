using HarmonyLib;
using Industrica.Utility;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(uGUI_Tooltip))]
    public static class GUITooltipPatches
    {
        [HarmonyPatch(nameof(uGUI_Tooltip.Set))]
        [HarmonyPostfix]
        public static void Set_Postfix(ITooltip tooltip)
        {
            uGUI_Tooltip.main.gameObject.EnsureComponent<GUIRecipeInformation>();

            if (tooltip is uGUI_RecipeEntry recipeEntry)
            {
                GUIRecipeInformation.Instance.Set(recipeEntry.techType);
            } else if (tooltip is uGUI_BlueprintEntry blueprintEntry
                && blueprintEntry.TryGetComponent(out GUIBlueprintEntryPatch.Holder blueprintHolder))
            {
                GUIRecipeInformation.Instance.Set(blueprintHolder.techType);
            } else
            {
                GUIRecipeInformation.Instance.Reset();
            }
        }

        [HarmonyPatch(nameof(uGUI_Tooltip.OnLayout))]
        [HarmonyPostfix]
        public static void OnLayout_Postfix()
        {
            if (GUIRecipeInformation.Instance == null)
            {
                return;
            }

            GUIRecipeInformation.Instance.OnLayout();
        }

        [HarmonyPatch(nameof(uGUI_Tooltip.UpdatePosition))]
        [HarmonyPostfix]
        public static void UpdatePosition_Postfix()
        {
            if (GUIRecipeInformation.Instance == null)
            {
                return;
            }

            GUIRecipeInformation.Instance.UpdatePosition();
        }
    }
}
