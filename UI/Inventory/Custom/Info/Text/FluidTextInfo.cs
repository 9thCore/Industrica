using Industrica.Utility;

namespace Industrica.UI.Inventory.Custom.Info.Text
{
    public class FluidTextInfo : FormatTextInfo
    {
        private int fill = 0;
        public int Fill
        {
            get
            {
                return fill;
            }
            set
            {
                if (fill == value)
                {
                    return;
                }

                fill = value;

                if (mesh != null)
                {
                    UpdateMeshText();
                }
            }
        }

        private int max = 0;
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                if (max == value)
                {
                    return;
                }

                max = value;

                if (mesh != null)
                {
                    UpdateMeshText();
                }
            }
        }

        private TechType fluid;
        public TechType Fluid
        {
            get
            {
                return fluid;
            }
            set
            {
                if (fluid == value)
                {
                    return;
                }

                fluid = value;

                if (mesh != null)
                {
                    UpdateMeshText();
                }
            }
        }

        public override void OnCreate()
        {
            mesh.fontSize = 32;
            mesh.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            mesh.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
        }

        public override void Update()
        {

        }

        protected override void UpdateMeshText()
        {
            mesh.text = Format.Translate(fill, max, fluid.AsString().Translate());
        }
    }
}
