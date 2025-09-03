using HarmonyLib;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnUpdate))]
    public static class InventoryTabPatch
    {
        public static void Postfix(uGUI_InventoryTab __instance)
        {
            foreach (uGUI_ItemsContainer container in __instance.torpedoStorage)
            {
                container.DoUpdate();
            }
        }
    }
}
