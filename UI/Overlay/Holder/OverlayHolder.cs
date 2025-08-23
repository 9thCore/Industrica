using UnityEngine;

namespace Industrica.UI.Overlay.Holder
{
    public abstract class OverlayHolder : MonoBehaviour
    {
        public abstract void Create(uGUI_ItemIcon icon);
    }
}
