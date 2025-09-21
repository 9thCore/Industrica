using UnityEngine;

namespace Industrica.UI.Inventory.Custom.Info.ItemIcon
{
    public class IconInfo : DisplayInfo
    {
        private uGUI_Icon icon;

        private Sprite sprite;
        public Sprite Sprite
        {
            get
            {
                return sprite;
            }
            set
            {
                if (sprite == value)
                {
                    return;
                }

                sprite = value;

                if (icon != null)
                {
                    icon.sprite = sprite;
                }
            }
        }

        public override void OnCreate()
        {

        }

        public override void OnSetPosition(Vector2 position)
        {
            if (icon != null)
            {
                icon.rectTransform.anchoredPosition = position;
            }
        }

        public override void OnSetRotation(float rotation)
        {
            if (icon != null)
            {
                icon.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            }
        }

        public override void OnSetScale(Vector2 scale)
        {
            if (icon != null)
            {
                icon.transform.localScale = scale;
            }
        }

        public override void SetActive(bool active)
        {
            if (icon != null)
            {
                icon.gameObject.SetActive(active);
            }
        }

        public override void Show()
        {
            if (UICustomDisplayHandler.Instance == null)
            {
                return;
            }

            if (icon == null)
            {
                GameObject go = UICustomDisplayHandler.Instance.GetNextChild();

                icon = go.EnsureComponent<uGUI_Icon>();
                icon.sprite = sprite;
                icon.rectTransform.anchoredPosition = Position;
                icon.transform.localScale = Scale;
                icon.transform.localRotation = Quaternion.Euler(0f, 0f, Rotation);

                OnCreate();
                Update();
            }

            UICustomDisplayHandler.Instance.Show(this);
        }

        public override void Update()
        {

        }
    }
}
