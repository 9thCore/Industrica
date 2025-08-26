using Industrica.Network.BaseModule;
using Industrica.Network.Container;
using Industrica.Network.Container.Provider;
using Industrica.Network.Filter;
using UnityEngine;

namespace Industrica.Network.Pipe.Item
{
    public class TransferItemPort : TransferPort<Pickupable>
    {
        public TransferItemPortHandler handler;
        public PhysicalItemPortRepresentation representation;

        private Container<Pickupable> itemContainer;
        public override Container<Pickupable> Container => itemContainer;

        public override PipeType AllowedPipeType => PipeType.Item;

        public static TransferItemPort CreatePort(GameObject prefab, GameObject root, Vector3 position, Quaternion rotation, PortType type, bool outside = false)
        {
            return CreatePort<TransferItemPort>(prefab, root, position, rotation, type, outside);
        }

        public override void CreateRepresentation(GameObject prefab, BaseModuleProvider provider)
        {
            representation = PhysicalItemPortRepresentation.Create(prefab, this, provider, gameObject);
        }

        public override void EnsureHandlerAndRegister(GameObject prefab, BaseModuleProvider provider)
        {
            handler = prefab.EnsureComponent<TransferItemPortHandler>();
            handler.Register(this);
            handler.WithBaseModule(provider);
        }

        public override void OnHoverStart()
        {
            representation.OnHoverStart();
        }

        public override void OnHover()
        {
            representation.OnHover();
        }

        public override void OnHoverEnd()
        {
            if (lockHover)
            {
                return;
            }

            representation.OnHoverEnd();
        }

        public override void Start()
        {
            base.Start();
            itemContainer = gameObject.GetComponentInParent<ContainerProvider<Pickupable>>().Container;
        }

        public override bool TryExtract(NetworkFilter<Pickupable> filter, out Pickupable value, bool simulate = false)
        {
            if (!IsOutput)
            {
                value = default;
                return false;
            }

            return itemContainer.TryExtract(filter, out value, simulate);
        }

        public override bool TryInsert(Pickupable value, bool simulate = false)
        {
            if (!IsInput)
            {
                return false;
            }

            return itemContainer.TryInsert(value, simulate);
        }
    }
}
