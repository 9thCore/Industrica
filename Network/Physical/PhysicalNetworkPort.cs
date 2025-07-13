using Industrica.ClassBase;
using Industrica.Item.Network;
using Industrica.Network.Container;
using Industrica.Network.Filter;
using Industrica.Network.Systems;
using Industrica.Save;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPort<T> : DestroyableMonoBehaviour, IPhysicalNetworkPort
    {
        public PortType port;
        public bool autoNetworkTransfer = false;
        private bool lockHover = false;
        private Transform parent;
        private UniqueIdentifier identifier, networkIdentifier;
        private PlacedTransferPipe connectedPipe = null;
        private PhysicalPortRepresentation physicalPort = null;
        private PhysicalNetwork<T> network;
        private PhysicalNetwork<T>.PhysicalConnection connection = null;
        private float elapsedSinceLastAutoTransfer = 0f;
        private NetworkFilter<T> insertFilter = null;

        public abstract Container<T> Container { get; }
        public virtual float AutoNetworkingInterval => 5f;

        public bool IsInput => port.HasFlag(PortType.Input);
        public bool IsOutput => port.HasFlag(PortType.Output);
        public bool CanInsert => IsInput;
        public bool CanExtract => IsOutput;
        public PortType Port => port;
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public Vector3 EndCapPosition => transform.position + transform.up * 0.06f;
        public Vector3 PipePosition => transform.position + transform.up * 0.2f;
        public bool Occupied => connectedPipe != null;
        public Transform Parent => parent;
        public string Id => identifier.Id;
        public string NetworkId => networkIdentifier.Id;
        public bool HasNetwork => network != null;
        public bool LockHover { set => lockHover = value; }

        protected static Derived CreatePort<Derived>(GameObject root, Vector3 position, Quaternion rotation, PortType type, bool autoNetworkTransfer) where Derived : PhysicalNetworkPort<T>
        {
            PhysicalNetworkPortHandler counter = root.EnsureComponent<PhysicalNetworkPortHandler>();

            GameObject portRoot = GameObjectUtil.CreateChild(root, typeof(Derived).Name, position: position, rotation: rotation);

            ChildObjectIdentifier identifier = portRoot.EnsureComponent<ChildObjectIdentifier>();
            identifier.ClassId = counter.GetClassID();

            Derived component = portRoot.EnsureComponent<Derived>();
            component.port = type;
            component.autoNetworkTransfer = autoNetworkTransfer;

            PhysicalPortRepresentation.CreatePort(portRoot);

            return component;
        }

        public virtual void Start()
        {
            parent = gameObject.TryGetComponentInParent(out Base seabase) ? seabase.transform : transform.parent;
            identifier = gameObject.GetComponent<UniqueIdentifier>();
            physicalPort = gameObject.GetComponentInChildren<PhysicalPortRepresentation>();
        }

        public virtual void Update()
        {
            UpdateAuto();
        }

        public virtual void OnDestroy()
        {
            if (connection == null)
            {
                return;
            }

            connection.Deregister();
        }

        public void Disconnect()
        {
            if (connectedPipe == null)
            {
                return;
            }

            connectedPipe.Disconnect();
            connectedPipe = null;
        }

        protected void UpdateAuto()
        {
            if (!autoNetworkTransfer || connection == null)
            {
                return;
            }

            elapsedSinceLastAutoTransfer += DayNightCycle.main.deltaTime;
            if (elapsedSinceLastAutoTransfer < AutoNetworkingInterval)
            {
                return;
            }
            elapsedSinceLastAutoTransfer -= AutoNetworkingInterval;

            InputFromNetwork();
            OutputIntoNetwork();
        }
        
        private void InputFromNetwork()
        {
            if (!port.HasFlag(PortType.Input))
            {
                return;
            }

            insertFilter ??= new InsertableNetworkFilter<T>(Container);
            if (!network.TryExtract(insertFilter, out T value))
            {
                return;
            }

            Container.TryInsert(value);
        }

        private void OutputIntoNetwork()
        {
            if (!port.HasFlag(PortType.Output))
            {
                return;
            }

            foreach (T value in Container)
            {
                if (network.TryInsert(value))
                {
                    return;
                }
            }
        }

        public virtual void SetNetwork<N>(N unvalidatedNetwork) 
        {
            if (unvalidatedNetwork is not PhysicalNetwork<T> network)
            {
                Plugin.Logger.LogError($"Attempted to set network that is not of type Network<{typeof(T).Name}>. Skipping");
                return;
            }

            this.network = network;
            networkIdentifier = network.GetComponent<UniqueIdentifier>();
            connection = network.Register(port, this);
        }

        public virtual System.Collections.IEnumerator SetNetworkID(string id)
        {
            yield return new WaitForSecondsRealtime(1f);

            if (!UniqueIdentifier.TryGetIdentifier(id, out UniqueIdentifier identifier))
            {
                Plugin.Logger.LogWarning($"No network found");
                yield break;
            }

            network = identifier.GetComponent<PhysicalNetwork<T>>();
            if (network != null)
            {
                SetNetwork(network);
            }
            
            Plugin.Logger.LogWarning($"Connected to network: {network}");
        }

        public virtual void Connect(PlacedTransferPipe pipe)
        {
            connectedPipe = pipe;
        }

        public virtual bool ShouldBeInteractable(TransferPipe pipe)
        {
            return !pipe.Holstering && AllowedPipeType == pipe.type && CanConnectTo(pipe);
        }

        public bool CanConnectTo(TransferPipe pipe)
        {
            return pipe.ConnectedTo(this) || port.HasFlag(pipe.NeededPort);
        }

        public  virtual void OnHoverStart()
        {
            physicalPort.OnHoverStart();
        }

        public virtual void OnHover()
        {
            physicalPort.OnHover();
        }

        public virtual void OnHoverEnd()
        {
            if (lockHover)
            {
                return;
            }

            physicalPort.OnHoverEnd();
        }

        public abstract TransferPipe.PipeType AllowedPipeType { get; }
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value);
        public abstract bool TryInsert(T value);

        public abstract class BaseSaveData<S, C> : ComponentSaveData<S, C> where S : BaseSaveData<S, C> where C : PhysicalNetworkPort<T>
        {
            public string networkId;
            public BaseSaveData(C component) : base(component) { }

            public override void CopyFromStorage(S data)
            {
                networkId = data.networkId;
            }

            public override void Load()
            {
                Component.SetNetworkID(networkId);
            }

            public override void Save()
            {
                networkId = Component.networkIdentifier.Id;
            }

            public override bool AbleToUpdateSave()
            {
                return base.AbleToUpdateSave() && Component.HasNetwork;
            }

            public override bool IncludeInSave()
            {
                return Component.HasNetwork;
            }
        }
    }
}
