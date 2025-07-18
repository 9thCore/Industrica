using System;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public class PhysicalPortRepresentation<T> : PortRepresentation<PhysicalNetworkPort<T>> where T : class
    {
        public override Color TargetColor => TargettedColor;

        public override Color NotTargetColor => UntargettedColor;

        public override Vector3 TargetSize => TargettedSize;

        public override Vector3 NotTargetSize => UntargettedSize;

        public override void OnHover()
        {
            if (parent == null)
            {
                return;
            }

            HandReticle.main.SetIcon(HandReticle.IconType.Hand);

            string type = parent.AllowedPipeType switch
            {
                PipeType.Item => "Item",
                _ => null
            };

            string port = parent.port switch
            {
                PortType.Input => "Input",
                PortType.Output => "Output",
                _ => null
            };

            HandReticle.main.SetText(HandReticle.TextType.Hand, $"IndustricaPipe_{type}_{port}", true, GameInput.Button.LeftHand);
        }

        public static readonly Vector3 UntargettedSize = new Vector3(0.16f, 0.25f, 0.16f);
        public static readonly Vector3 TargettedSize = new Vector3(0.25f, 0.37f, 0.25f);
        public static readonly Color UntargettedColor = Color.white;
        public static readonly Color TargettedColor = Color.blue;
    }
}
