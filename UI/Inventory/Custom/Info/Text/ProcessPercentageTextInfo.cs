using Industrica.Utility;

namespace Industrica.UI.Inventory.Custom.Info.Text
{
    public class ProcessPercentageTextInfo : FormatTextInfo
    {
        private int percentage = 0;
        public int Percentage
        {
            get
            {
                return percentage;
            }
            set
            {
                if (percentage == value)
                {
                    return;
                }

                percentage = value;

                if (mesh != null)
                {
                    UpdateMeshText();
                }
            }
        }

        public override void OnCreate()
        {
            mesh.fontSize = 20;
            mesh.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            mesh.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
            mesh.rectTransform.sizeDelta = UIUtil.ProcessProgressBarSize;
        }

        public override void Update()
        {

        }

        protected override void UpdateMeshText()
        {
            mesh.text = Format.Translate(percentage);
        }
    }
}
