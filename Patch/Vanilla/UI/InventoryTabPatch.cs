using HarmonyLib;
using Industrica.UI.Inventory.Custom;
using Industrica.Utility;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(uGUI_InventoryTab))]
    public static class InventoryTabPatch
    {
        [HarmonyPatch(nameof(uGUI_InventoryTab.OnUpdate))]
        [HarmonyPostfix]
        public static void OnUpdate(uGUI_InventoryTab __instance)
        {
            if (UICustomContainerHandler.Instance == null)
            {
                return;
            }

            UICustomContainerHandler.Instance.DoUpdate();
        }

        [HarmonyPatch(nameof(uGUI_InventoryTab.OnClosePDA))]
        [HarmonyPostfix]
        public static void OnClosePDA()
        {
            if (UICustomContainerHandler.Instance != null)
            {
                UICustomContainerHandler.Instance.Revert();
            }

            if (UICustomDisplayHandler.Instance != null)
            {
                UICustomDisplayHandler.Instance.Revert();
            }
        }

        [HarmonyPatch(nameof(uGUI_InventoryTab.Start))]
        [HarmonyPostfix]
        public static void Start(uGUI_InventoryTab __instance)
        {
            __instance.gameObject.EnsureComponent<UICustomContainerHandler>();
            __instance.gameObject.EnsureComponent<UICustomDisplayHandler>();
        }
    }
}
