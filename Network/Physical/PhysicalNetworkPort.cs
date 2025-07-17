using Industrica.Item.Network.Placed;
using Industrica.Network.Container;
using Industrica.Network.Filter;
using Industrica.Network.Systems;
using Industrica.Save;
using Industrica.Utility;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPort<T> : MonoBehaviour where T : class
    {
        private bool lockHover = false;

        public UniqueIdentifier identifier;
        public bool hasPumpModule = false;
        public PortType port;

        internal PhysicalNetworkPort<T> connectedPort = null;
        internal Transform parent = null;
        internal PhysicalNetwork<T> network;
        internal UniqueIdentifier networkIdentifier;
        internal PhysicalPortRepresentation<T> physicalPort = null;
        internal PhysicalNetwork<T>.PhysicalConnection connection = null;
        internal PhysicalNetworkPortPump<T> pump;
        internal PlacedTransferPipe<T> transferPipe = null;

        public abstract Container<T> Container { get; }
        public abstract PipeType AllowedPipeType { get; }
        public abstract void CreateAndSetNetwork(Action<PhysicalNetwork<T>> action);
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false);
        public abstract bool TryInsert(T value, bool simulate = false);

        public bool IsInput => port == PortType.Input;
        public bool IsOutput => port == PortType.Output;
        public Vector3 EndCapPosition => transform.position + transform.up * 0.06f;
        public Vector3 PipePosition => transform.position + transform.up * 0.2f;
        public string Id => identifier.Id;
        public bool HasNetwork => network != null;
        public bool LockHover { set => lockHover = value; }

        protected static P CreatePort<P, H, R>(GameObject root, Vector3 position, Quaternion rotation, PortType type, bool autoNetworkTransfer)
            where P : PhysicalNetworkPort<T>
            where H : PhysicalNetworkPortHandler<T>
            where R : PhysicalPortRepresentation<T>
        {
            H handler = root.EnsureComponent<H>();

            GameObject portRoot = GameObjectUtil.CreateChild(root, typeof(P).Name, position: position, rotation: rotation);

            ChildObjectIdentifier identifier = portRoot.EnsureComponent<ChildObjectIdentifier>();
            identifier.ClassId = handler.GetClassID();

            P component = portRoot.EnsureComponent<P>();
            component.port = type;
            component.hasPumpModule = autoNetworkTransfer;
            component.identifier = component.GetComponent<UniqueIdentifier>();

            PhysicalPortRepresentation<T>.CreatePort<R>(portRoot);

            return component;
        }

        public virtual void Start()
        {
            parent = gameObject.TryGetComponentInParent(out SubRoot seabase) ? seabase.transform : transform.parent;
            physicalPort = GetComponentInChildren<PhysicalPortRepresentation<T>>();

            if (hasPumpModule)
            {
                pump = new PhysicalNetworkPortPump<T>(this);
            }
        }

        public void OnDestroy()
        {
            NetworkDisconnect();
        }

        public virtual void NetworkDisconnect()
        {
            if (transferPipe == null)
            {
                return;
            }

            PlacedTransferPipe<T> copy = transferPipe;
            transferPipe = null;
            copy.Disconnect();
            pump?.NetworkDisconnect();
            connection.Deregister();
            connection = null;
            network = null;
            networkIdentifier = null;
        }

        public virtual void SetNetwork(PhysicalNetwork<T> network)
        {
            this.network = network;
            pump?.SetNetwork(network);
            networkIdentifier = network.GetComponent<UniqueIdentifier>();
            connection = network.Register(port, this);
        }

        public IEnumerator SetNetworkID(string id)
        {
            yield return new WaitForSecondsRealtime(1f);

            if (!UniqueIdentifier.TryGetIdentifier(id, out UniqueIdentifier identifier))
            {
                Plugin.Logger.LogError($"No network ({id}) found. Cannot connect");
                yield break;
            }

            network = identifier.GetComponent<PhysicalNetwork<T>>();
            if (network != null)
            {
                SetNetwork(network);
            }
        }

        public void Connect(PlacedTransferPipe<T> pipe)
        {
            transferPipe = pipe;
        }

        public void Connect(PhysicalNetworkPort<T> port)
        {
            connectedPort = port;
        }

        public bool ShouldBeInteractable(TransferPipe<T> pipe)
        {
            return !pipe.Holstering && AllowedPipeType == pipe.Type && CanConnectTo(pipe);
        }

        public bool CanConnectTo(TransferPipe<T> pipe)
        {
            return pipe.ConnectedTo(this) || port.HasFlag(pipe.neededPort);
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
                CoroutineHost.StartCoroutine(Component.SetNetworkID(networkId));
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
