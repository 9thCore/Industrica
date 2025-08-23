using Industrica.Network.Filter.Holder;
using Industrica.Utility;

namespace Industrica.UI.Overlay
{
    public class ItemTechTypeFilterOverlay : AbstractOverlay
    {
        public TechTypeNetworkFilterHolder holder;
        private TechType lastTechType;

        public static ItemTechTypeFilterOverlay Create(uGUI_ItemIcon itemIcon, string name, TechTypeNetworkFilterHolder holder)
        {
            ItemTechTypeFilterOverlay overlay = itemIcon.foreground.gameObject.CreateChild(name)
                .EnsureComponent<ItemTechTypeFilterOverlay>();

            overlay.icon = overlay.gameObject.EnsureComponent<uGUI_Icon>();
            overlay.holder = holder;

            overlay.UpdateIcon(holder.TechType);

            return overlay;
        }

        public void Update()
        {
            if (holder.TechType == lastTechType)
            {
                return;
            }

            UpdateIcon(holder.TechType);
        }

        private void UpdateIcon(TechType techType)
        {
            if (techType == TechType.None)
            {
                icon.enabled = false;
                return;
            }

            icon.enabled = true;
            icon.sprite = SpriteManager.Get(techType);

            lastTechType = holder.TechType;
        }
    }
}
