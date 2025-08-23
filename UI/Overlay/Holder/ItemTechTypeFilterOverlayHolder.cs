using Industrica.Network.Filter.Holder;
using UnityEngine;

namespace Industrica.UI.Overlay.Holder
{
    public class ItemTechTypeFilterOverlayHolder : OverlayHolder
    {
        public TechTypeNetworkFilterHolder holder;

        public void WithNetworkFilterHolder(TechTypeNetworkFilterHolder holder)
        {
            this.holder = holder;
        }

        public override void CreateOrUpdate(uGUI_ItemIcon icon)
        {
            if (!TryGetComponent(out TechTypeNetworkFilterHolder holder))
            {
                Plugin.Logger.LogError($"Could not find {nameof(TechTypeNetworkFilterHolder)} in {gameObject}. Disabling...");
                enabled = false;
                return;
            }

            ItemTechTypeFilterOverlay overlay = icon.gameObject.GetComponentInChildren<ItemTechTypeFilterOverlay>();
            if (overlay != null)
            {
                overlay.SetHolder(holder);
                return;
            }

            overlay = ItemTechTypeFilterOverlay.Create(icon, ChildName, holder);

            overlay.transform.localScale = OverlayScale;
            overlay.icon.rectTransform.anchorMin = OverlayAnchor;
            overlay.icon.rectTransform.anchorMax = OverlayAnchor;

            overlay.SetColor(OverlayColor);
        }

        public const string ChildName = $"Industrica{nameof(ItemTechTypeFilterOverlay)}";
        public static readonly Color OverlayColor = new(1f, 1f, 1f, 0.75f);
        public static readonly Vector3 OverlayScale = Vector3.one * 0.75f;
        public static readonly Vector3 OverlayAnchor = new Vector2(0.6f, 0.4f);
    }
}
