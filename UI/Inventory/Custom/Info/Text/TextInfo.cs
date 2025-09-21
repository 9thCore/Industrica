using Industrica.Utility;
using TMPro;
using UnityEngine;

namespace Industrica.UI.Inventory.Custom.Info.Text
{
    public abstract class TextInfo : DisplayInfo
    {
        protected TextMeshProUGUI mesh;

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text == value)
                {
                    return;
                }

                text = value;

                if (mesh != null)
                {
                    UpdateMeshText();
                }
            }
        }

        public override void OnSetPosition(Vector2 position)
        {
            if (mesh != null)
            {
                mesh.rectTransform.anchoredPosition = position;
            }
        }

        public override void OnSetScale(Vector2 scale)
        {
            if (mesh != null)
            {
                mesh.transform.localScale = scale;
            }
        }

        public override void OnSetRotation(float rotation)
        {
            if (mesh != null)
            {
                mesh.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            }
        }

        public override void SetActive(bool active)
        {
            if (mesh != null)
            {
                mesh.gameObject.SetActive(active);
            }
        }

        public override void Show()
        {
            if (UICustomDisplayHandler.Instance == null)
            {
                return;
            }

            if (mesh == null)
            {
                GameObject go = UICustomDisplayHandler.Instance.GetNextChild();

                mesh = go.EnsureComponent<TextMeshProUGUI>();
                mesh.rectTransform.anchoredPosition = Position;
                mesh.transform.localScale = Scale;
                mesh.transform.localRotation = Quaternion.Euler(0f, 0f, Rotation);

                OnCreate();
                Update();
                UpdateMeshText();
            }

            UICustomDisplayHandler.Instance.Show(this);
        }

        private void UpdateMeshText()
        {
            mesh.text = text.Translate();
        }
    }
}
