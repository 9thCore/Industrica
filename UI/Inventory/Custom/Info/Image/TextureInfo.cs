using UnityEngine;

namespace Industrica.UI.Inventory.Custom.Info.Image
{
    public class TextureInfo : ImageInfo
    {
        private Texture2D texture;
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                if (texture == value)
                {
                    return;
                }

                if (image != null)
                {
                    image.material.mainTexture = value;
                }

                texture = value;
            }
        }

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

        public override void OnCreate()
        {
            image.material.mainTexture = texture;
        }

        public override void Update()
        {
            
        }
    }
}
