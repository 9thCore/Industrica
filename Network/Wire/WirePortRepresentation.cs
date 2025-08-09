using Industrica.Network.BaseModule;
using UnityEngine;

namespace Industrica.Network.Wire
{
    public class WirePortRepresentation : PortRepresentation<WirePort>
    {
        public override Color TargetColor => TargettedColor;

        public override Color NotTargetColor => UntargettedColor;

        public override Vector3 TargetSize => TargettedSize;

        public override Vector3 NotTargetSize => UntargettedSize;

        public static WirePortRepresentation Create(GameObject prefab, WirePort parent, BaseModuleProvider provider, GameObject portRoot)
        {
            return Create<WirePortRepresentation>(prefab, parent, provider, portRoot);
        }

        public void OnEnable()
        {
            WireTool.OnConnectionRefresh += RefreshConnection;
        }

        public void OnDisable()
        {
            WireTool.OnConnectionRefresh -= RefreshConnection;
        }

        public void RefreshConnection(WireTool wire)
        {
            if (Constructed < 1f
                || parent == null)
            {
                interactableGO.SetActive(false);
                return;
            }

            if (!parent.ShouldBeInteractable(wire))
            {
                interactableGO.SetActive(false);
                OnHoverEnd();
                return;
            }

            interactableGO.SetActive(true);
        }

        public override void OnHover()
        {
            if (parent == null)
            {
                return;
            }

            HandReticle.main.SetIcon(HandReticle.IconType.Hand);

            string port = parent.port switch
            {
                PortType.Input => "Input",
                PortType.Output => "Output",
                _ => null
            };

            HandReticle.main.SetText(HandReticle.TextType.Hand, $"IndustricaWire_{port}", true, GameInput.Button.LeftHand);
        }

        public static readonly Vector3 UntargettedSize = new Vector3(0.1f, 0.16f, 0.1f);
        public static readonly Vector3 TargettedSize = new Vector3(0.16f, 0.25f, 0.16f);
        public static readonly Color UntargettedColor = Color.white;
        public static readonly Color TargettedColor = Color.red;
    }
}
