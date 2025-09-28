using HarmonyLib;
using Industrica.UI.Overlay;
using Industrica.UI.Overlay.Holder;

namespace Industrica.Patch.Vanilla.UI
{
    public static class ItemOverlayPatch
    {
        [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.OnAddItem))]
        public static class ItemsContainerPatch
        {
            public static void Postfix(InventoryItem item, uGUI_ItemsContainer __instance)
            {
                if (!__instance.items.TryGetValue(item, out uGUI_ItemIcon icon))
                {
                    return;
                }

                if (item.item.TryGetComponent(out OverlayHolder holder))
                {
                    holder.CreateOrUpdate(icon);
                } else
                {
                    RemoveOverlay(icon);
                }
            }
        }

        [HarmonyPatch(typeof(ItemDragManager), nameof(ItemDragManager.InternalDragStart))]
        public static class ItemDragManagerPatch
        {
            public static void Postfix(InventoryItem item, ItemDragManager __instance)
            {
                if (item.item.TryGetComponent(out OverlayHolder holder))
                {
                    holder.CreateOrUpdate(__instance.draggedIcon);
                } else
                {
                    RemoveOverlay(__instance.draggedIcon);
                }
            }
        }

        [HarmonyPatch(typeof(uGUI_EquipmentSlot), nameof(uGUI_EquipmentSlot.SetItem))]
        public static class EquipmentSlotPatch
        {
            public static void Postfix(InventoryItem item, uGUI_EquipmentSlot __instance)
            {
                if (item.item.TryGetComponent(out OverlayHolder holder))
                {
                    holder.CreateOrUpdate(__instance.icon);
                } else
                {
                    RemoveOverlay(__instance.icon);
                }
            }
        }

        private static void RemoveOverlay(uGUI_ItemIcon icon)
        {
            AbstractOverlay overlay = icon.GetComponentInChildren<AbstractOverlay>(true);
            if (overlay != null)
            {
                overlay.Remove();
            }
        }
    }
}
