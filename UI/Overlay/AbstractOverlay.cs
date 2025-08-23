using UnityEngine;

namespace Industrica.UI.Overlay
{
    public abstract class AbstractOverlay : MonoBehaviour
    {
        protected uGUI_Icon icon;

        public void SetColor(Color color)
        {
            if (TryGetComponent(out CanvasRenderer renderer))
            {
                renderer.SetColor(color);
            }
        }
    }
}
