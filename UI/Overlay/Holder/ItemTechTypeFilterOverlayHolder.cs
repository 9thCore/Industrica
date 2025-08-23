using Industrica.Network.Filter.Holder;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.UI.Overlay.Holder
{
    public class ItemTechTypeFilterOverlayHolder : OverlayHolder
    {
        public TechTypeNetworkFilterHolder holder;
        private ItemTechTypeFilterOverlay overlay;

        public void WithNetworkFilterHolder(TechTypeNetworkFilterHolder holder)
        {
            this.holder = holder;
        }

        public override void Create(uGUI_ItemIcon icon)
        {
            if (!TryGetComponent(out TechTypeNetworkFilterHolder holder))
            {
                Plugin.Logger.LogError($"Could not find {nameof(TechTypeNetworkFilterHolder)} in {gameObject}. Disabling...");
                enabled = false;
                return;
            }

            overlay = ItemTechTypeFilterOverlay.Create(icon, $"Industrica{nameof(ItemTechTypeFilterOverlay)}", holder);

            overlay.transform.localScale = OverlayScale;
            overlay.transform.localPosition = OverlayPosition;

            overlay.SetColor(OverlayColor);
        }

        public static readonly Color OverlayColor = new(1f, 1f, 1f, 0.75f);
        public static readonly Vector3 OverlayScale = Vector3.one * 0.5f;
        public static readonly Vector3 OverlayPosition = new Vector3(8f, -8f, 0f);
    }
}
