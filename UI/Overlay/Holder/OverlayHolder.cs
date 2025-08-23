using UnityEngine;

namespace Industrica.UI.Overlay.Holder
{
    public abstract class OverlayHolder : MonoBehaviour
    {
        public abstract void CreateOrUpdate(uGUI_ItemIcon icon);
    }
}
