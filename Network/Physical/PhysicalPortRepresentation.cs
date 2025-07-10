using Industrica.Item.Network;
using Industrica.Utility;
using Nautilus.Utility;
using rail;
using System;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public class PhysicalPortRepresentation : MonoBehaviour
    {
        private readonly SmoothValue hoverInterpolation = new(initialDuration: 0.25f);
        private Transform interactable;
        private GameObject interactableGO;
        private Renderer renderer;
        private IPhysicalNetworkPort parent;
        private Constructable constructable;

        public static void CreatePort(GameObject portRoot)
        {
            GameObject representation = GameObjectUtil.CreateChild(portRoot, nameof(PhysicalPortRepresentation));
            representation.EnsureComponent<PhysicalPortRepresentation>();

            GameObject interactable = GameObjectUtil.CreateChild(representation, "Interactable", primitive: PrimitiveType.Cube);
            interactable.SetActive(false);
            interactable.layer = LayerID.Useable;
            MaterialUtils.ApplySNShaders(interactable, 8f, 0f, 0f);

            interactable.EnsureComponent<GenericHandTarget>();
            interactable.EnsureComponent<SkyApplier>().renderers = interactable.GetComponents<Renderer>();
        }

        public void Start()
        {
            parent = GetComponentInParent<IPhysicalNetworkPort>();

            renderer = GetComponentInChildren<Renderer>(true);
            interactable = renderer.transform;
            interactableGO = renderer.gameObject;

            constructable = GetComponentInParent<Constructable>();

            hoverInterpolation.OnFinish += HoverAnimationFinish;

            Animate(0f);
        }

        public void Update()
        {
            hoverInterpolation.Update();
            if (hoverInterpolation.IsChanging())
            {
                Animate(hoverInterpolation.value);
            }
        }

        public void OnEnable()
        {
            TransferPipe.OnConnectionRefresh += RefreshConnection;
        }

        public void OnDisable()
        {
            TransferPipe.OnConnectionRefresh -= RefreshConnection;
        }

        public void RefreshConnection(TransferPipe pipe)
        {
            if (!constructable.constructed
                || parent.IsDestroyed())
            {
                interactableGO.SetActive(false);
                return;
            }

            if (!parent.ShouldBeInteractable(pipe))
            {
                interactableGO.SetActive(false);
                OnHoverEnd();
                return;
            }

            interactableGO.SetActive(true);
        }

        private void HoverAnimationFinish(float value)
        {
            Animate(value);
        }

        public void Animate(float progress)
        {
            interactable.localScale = Vector3.Lerp(UntargettedSize, TargettedSize, progress);
            renderer.material.color = Color.Lerp(UntargettedColor, TargettedColor, progress);
        }

        public void OnHoverStart()
        {
            hoverInterpolation.SetTarget(1f);
        }

        public void OnHoverEnd()
        {
            hoverInterpolation.SetTarget(0f);
        }

        public void OnHover()
        {
            if (parent.IsDestroyed())
            {
                return;
            }

            HandReticle.main.SetIcon(HandReticle.IconType.Hand);

            string text = parent.AllowedPipeType switch
            {
                TransferPipe.PipeType.Item => "IndustricaPipe_Item",
                _ => null
            };

            if (!string.IsNullOrEmpty(text))
            {
                HandReticle.main.SetText(HandReticle.TextType.Hand, text, true, GameInput.Button.LeftHand);
            }

            string subscript = parent.Port switch
            {
                PortType.Input => "IndustricaPipe_Input",
                PortType.Output => "IndustricaPipe_Output",
                PortType.InputAndOutput => "IndustricaPipe_InputAndOutput",
                _ => null
            };

            if (!string.IsNullOrEmpty(subscript))
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, subscript, true);
            }
        }

        public static readonly Vector3 UntargettedSize = new Vector3(0.16f, 0.25f, 0.16f);
        public static readonly Vector3 TargettedSize = new Vector3(0.25f, 0.37f, 0.25f);
        public static readonly Color UntargettedColor = Color.white;
        public static readonly Color TargettedColor = Color.blue;
    }
}
