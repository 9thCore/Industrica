using UnityEngine;

namespace Industrica.UI.Inventory.Custom.Info.Image
{
    public class ProcessProgressBarTextureInfo : ImageInfo
    {
        private Color color;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color == value)
                {
                    return;
                }

                color = value;

                if (image != null)
                {
                    UpdateColorValue();
                }
            }
        }

        private float percentage = 0;
        public float Percentage
        {
            get
            {
                return percentage;
            }
            set
            {
                percentage = Mathf.Clamp01(value);

                if (image != null)
                {
                    UpdateProgressValue();
                }
            }
        }

        public override void OnCreate()
        {
            image.material = new(image.material)
            {
                shader = ShaderManager.preloadedShaders.UICircularBar
            };

            image.material.SetFloat(ShaderPropertyID._Width, 0.2f);
            UpdateProgressValue();
            UpdateColorValue();
        }

        public override void Update()
        {

        }

        private void UpdateProgressValue()
        {
            image.material.SetFloat(ShaderPropertyID._Value, percentage);
        }

        private void UpdateColorValue()
        {
            image.material.SetColor(ShaderPropertyID._Color, color);
        }
    }
}
