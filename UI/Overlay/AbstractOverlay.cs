using UnityEngine;

namespace Industrica.UI.Overlay
{
    public abstract class AbstractOverlay : MonoBehaviour
    {
        public uGUI_Icon icon;
        protected uGUI_Icon parent;

        public void SetColor(Color color)
        {
            if (TryGetComponent(out CanvasRenderer renderer))
            {
                renderer.SetColor(color);
            }
        }

        public virtual void Update()
        {
            icon.rectTransform.sizeDelta = parent.rectTransform.sizeDelta;
        }
    }
}
