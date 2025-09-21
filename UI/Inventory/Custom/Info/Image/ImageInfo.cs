using UnityEngine;

namespace Industrica.UI.Inventory.Custom.Info.Image
{
    public abstract class ImageInfo : DisplayInfo
    {
        protected UnityEngine.UI.Image image;

        public override void SetActive(bool active)
        {
            if (image != null)
            {
                image.gameObject.SetActive(active);
            }
        }

        public override void OnSetPosition(Vector2 position)
        {
            if (image != null)
            {
                image.rectTransform.anchoredPosition = position;
            }
        }

        public override void OnSetScale(Vector2 scale)
        {
            if (image != null)
            {
                image.transform.localScale = scale;
            }
        }

        public override void OnSetRotation(float rotation)
        {
            if (image != null)
            {
                image.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            }
        }

        public override void Show()
        {
            if (UICustomDisplayHandler.Instance == null)
            {
                return;
            }

            if (image == null)
            {
                GameObject go = UICustomDisplayHandler.Instance.GetNextChild();

                image = go.EnsureComponent<UnityEngine.UI.Image>();
                image.rectTransform.anchoredPosition = Position;
                image.transform.localScale = Scale;
                image.transform.localRotation = Quaternion.Euler(0f, 0f, Rotation);

                OnCreate();
                Update();
            }

            UICustomDisplayHandler.Instance.Show(this);
        }
    }
}
