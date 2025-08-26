using Industrica.Network.BaseModule;
using Industrica.Network.Container;
using Industrica.Network.Container.Provider;
using Industrica.Network.Filter;
using Industrica.Network.Systems;
using Industrica.Save;
using System;
using UnityEngine;
using UWE;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalNetworkItemPort : PhysicalNetworkPort<Pickupable>
    {
        public PhysicalNetworkItemPortHandler handler;
        public PhysicalItemPortRepresentation representation;

        private Container<Pickupable> itemContainer;
        public override Container<Pickupable> Container => itemContainer;

        public override PipeType AllowedPipeType => PipeType.Item;

        public static PhysicalNetworkItemPort CreatePort(GameObject prefab, GameObject root, Vector3 position, Quaternion rotation, PortType type, bool outside = false)
        {
            return CreatePort<PhysicalNetworkItemPort>(prefab, root, position, rotation, type, outside);
        }

        public override void CreateAndSetNetwork(Action<PhysicalNetwork<Pickupable>> action)
        {
            CoroutineHost.StartCoroutine(ItemPhysicalNetwork.Create(network =>
            {
                SetNetwork(network);
                action.Invoke(network);
            }));
        }

        public override void CreateRepresentation(GameObject prefab, BaseModuleProvider provider)
        {
            representation = PhysicalItemPortRepresentation.Create(prefab, this, provider, gameObject);
        }

        public override void EnsureHandlerAndRegister(GameObject prefab, BaseModuleProvider provider)
        {
            handler = prefab.EnsureComponent<PhysicalNetworkItemPortHandler>();
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
            new SaveData(this);
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

        public class SaveData : BaseSaveData<SaveData, PhysicalNetworkItemPort>
        {
            public SaveData(PhysicalNetworkItemPort component) : base(component) { }
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.physicalNetworkItemPortData;
        }
    }
}
