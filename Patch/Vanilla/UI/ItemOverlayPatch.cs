using HarmonyLib;
using Industrica.UI.Overlay.Holder;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.OnAddItem))]
    public static class ItemOverlayPatch
    {
        public static void Postfix(InventoryItem item, uGUI_ItemsContainer __instance)
        {
            if (item.item.TryGetComponent(out OverlayHolder holder))
            {
                holder.Create(__instance.items[item]);
            }
        }
    }
}
