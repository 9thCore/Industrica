using UnityEngine;

namespace Industrica.UI.Inventory.Custom.Info.Image
{
    public class SpriteInfo : ImageInfo
    {
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

                if (image != null)
                {
                    image.sprite = value;
                }

                sprite = value;
            }
        }

        public override void OnCreate()
        {
            image.sprite = sprite;
        }

        public override void Update()
        {
            
        }
    }
}
