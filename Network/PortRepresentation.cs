using Industrica.Network.BaseModule;
using Industrica.Utility;
using Industrica.Utility.Smooth;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class PortRepresentation<T> : MonoBehaviour where T : MonoBehaviour
    {
        public Renderer renderer;
        public T parent;
        public Constructable constructable;
        public BaseModuleProvider provider;

        protected readonly FloatSmoothValue hoverInterpolation = new(initialDuration: 0.25f);
        protected Transform interactable;
        protected GameObject interactableGO;

        protected float Constructed
        {
            get
            {
                if (constructable != null)
                {
                    return constructable.constructedAmount;
                }

                if (provider != null)
                {
                    return provider.ConstructedAmount;
                }

                return 0f;
            }
        }

        public static P Create<P>(GameObject prefab, T parent, BaseModuleProvider provider, GameObject portRoot) where P : PortRepresentation<T>
        {
            GameObject representation = portRoot.CreateChild(typeof(P).Name);

            GameObject interactable = representation.CreateChild("Interactable", primitive: PrimitiveType.Cube);
            interactable.SetActive(false);
            interactable.GetComponent<Collider>().isTrigger = true;
            MaterialUtils.ApplySNShaders(interactable, 8f, 0f, 0f);

            interactable.EnsureComponent<GenericHandTarget>();
            interactable.EnsureComponent<SkyApplier>().renderers = interactable.GetComponents<Renderer>();

            P component = representation.EnsureComponent<P>();
            component.renderer = representation.GetComponentInChildren<Renderer>(true);
            component.parent = parent;

            if (provider != null)
            {
                component.provider = provider;
            } else
            {
                component.constructable = prefab.GetComponent<Constructable>();
            }

            return component;
        }

        public abstract Color TargetColor { get; }
        public abstract Color NotTargetColor { get; }
        public abstract Vector3 TargetSize { get; }
        public abstract Vector3 NotTargetSize { get; }
        public abstract void OnHover();

        public void Start()
        {
            interactable = renderer.transform;
            interactableGO = renderer.gameObject;

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

        private void HoverAnimationFinish(float value)
        {
            Animate(value);
        }

        public void Animate(float progress)
        {
            interactable.localScale = Vector3.Lerp(NotTargetSize, TargetSize, progress);
            renderer.material.color = Color.Lerp(NotTargetColor, TargetColor, progress);
        }

        public void OnHoverStart()
        {
            hoverInterpolation.SetTarget(1f);
        }

        public void OnHoverEnd()
        {
            hoverInterpolation.SetTarget(0f);
        }

        public static readonly int Layer = LayerID.Useable;
    }
}
