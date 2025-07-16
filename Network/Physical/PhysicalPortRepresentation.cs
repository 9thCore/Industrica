using Industrica.Item.Network;
using Industrica.Network.BaseModule;
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
        private IBaseModuleConstructionProvider provider;

        private float Constructed
        {
            get
            {
                if (constructable != null)
                {
                    return constructable.constructedAmount;
                }

                return provider.ConstructedAmount;
            }
        }

        public static void CreatePort(GameObject portRoot)
        {
            GameObject representation = GameObjectUtil.CreateChild(portRoot, nameof(PhysicalPortRepresentation));
            representation.EnsureComponent<PhysicalPortRepresentation>();

            GameObject interactable = GameObjectUtil.CreateChild(representation, "Interactable", primitive: PrimitiveType.Cube);
            interactable.SetActive(false);
            interactable.GetComponent<Collider>().isTrigger = true;
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

            if (constructable == null)
            {
                provider = GetComponentInParent<IBaseModuleConstructionProvider>();
                if (provider == null)
                {
                    gameObject.SetActive(false);
                    throw new InvalidOperationException();
                }
            }

            hoverInterpolation.OnFinish += HoverAnimationFinish;

            Animate(0f);
        }

        public void Update()
        {
            // lol
            if (interactableGO.layer != Layer)
            {
                interactableGO.layer = Layer;
            }

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
            if (Constructed < 1f
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
            renderer.material.color = Color.Lerp(GetUntargetColor(), GetTargetColor(), progress);
        }

        private Color GetUntargetColor()
        {
            if (parent.IsDestroyed())
            {
                return UntargettedColor;
            }

            return parent.AutoNetwork ? UntargettedAutoColor : UntargettedColor;
        }

        private Color GetTargetColor()
        {
            if (parent.IsDestroyed())
            {
                return TargettedColor;
            }

            return parent.AutoNetwork ? TargettedAutoColor : TargettedColor;
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

            string type = parent.AllowedPipeType switch
            {
                TransferPipe.PipeType.Item => "Item",
                _ => null
            };

            string port = parent.Port switch
            {
                PortType.Input => "Input",
                PortType.Output => "Output",
                _ => null
            };

            HandReticle.main.SetText(HandReticle.TextType.Hand, $"IndustricaPipe_{type}_{port}", true, GameInput.Button.LeftHand);

            if (parent.AutoNetwork)
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, $"IndustricaPipe_Auto{port}", true);
            }
        }

        public static readonly int Layer = LayerID.Useable;

        public static readonly Vector3 UntargettedSize = new Vector3(0.16f, 0.25f, 0.16f);
        public static readonly Vector3 TargettedSize = new Vector3(0.25f, 0.37f, 0.25f);
        public static readonly Color UntargettedColor = Color.white;
        public static readonly Color UntargettedAutoColor = Color.yellow;
        public static readonly Color TargettedColor = Color.blue;
        public static readonly Color TargettedAutoColor = Color.red;
    }
}
