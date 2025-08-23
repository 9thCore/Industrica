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

        public void Start()
        {
            UpdateSizeDelta();
        }

        public virtual void Update()
        {
            UpdateSizeDelta();
        }

        public void UpdateSizeDelta()
        {
            icon.rectTransform.sizeDelta = parent.rectTransform.sizeDelta;
        }
    }
}
