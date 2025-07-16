using Industrica.ClassBase;
using Industrica.Item.Network.Placed;
using Industrica.Network.Container;
using Industrica.Network.Filter;
using Industrica.Network.Systems;
using Industrica.Save;
using Industrica.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPort<T> : MonoBehaviour
    {
        public PortType port;
        public bool autoNetworkTransfer = false;
        public PhysicalNetworkPort<T> ConnectedPort { get; private set; } = null;

        private bool lockHover = false;
        private Transform parent;
        private NetworkFilter<T> insertFilter, extractFilter = null;
        private PhysicalNetworkPortHandler<T> handler;
        private UniqueIdentifier identifier, networkIdentifier;
        private PlacedTransferPipe<T> connectedPipe = null;
        private PhysicalPortRepresentation<T> physicalPort = null;
        private PhysicalNetwork<T> network;
        private PhysicalNetwork<T>.PhysicalConnection connection = null;
        private IEnumerable<PhysicalNetworkPort<T>> siblings = null;

        public abstract Container<T> Container { get; }
        public virtual float AutoNetworkingInterval => 5f;

        public bool IsInput => port.HasFlag(PortType.Input);
        public bool IsOutput => port.HasFlag(PortType.Output);
        public PortType Port => port;
        public Transform Transform => transform;
        public Vector3 EndCapPosition => transform.position + transform.up * 0.06f;
        public Vector3 PipePosition => transform.position + transform.up * 0.2f;
        public bool Occupied => connectedPipe != null;
        public Transform Parent => parent;
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
            component.autoNetworkTransfer = autoNetworkTransfer;

            PhysicalPortRepresentation<T>.CreatePort<R>(portRoot);

            return component;
        }

        public virtual void Start()
        {
            parent = gameObject.TryGetComponentInParent(out SubRoot seabase) ? seabase.transform : transform.parent;
            identifier = gameObject.GetComponent<UniqueIdentifier>();
            physicalPort = gameObject.GetComponentInChildren<PhysicalPortRepresentation<T>>();
            handler = gameObject.GetComponentInParent<PhysicalNetworkPortHandler<T>>();
        }

        public virtual void OnDestroy()
        {
            NetworkDisconnect();
        }

        public void Disconnect()
        {
            if (connectedPipe == null)
            {
                return;
            }

            ConnectedPort = null;
            
            connectedPipe.Disconnect();
            connectedPipe = null;

            NetworkDisconnect();
        }

        public void NetworkDisconnect()
        {
            if (connection == null)
            {
                return;
            }

            network.OnPump -= UpdateAuto;
            connection.Deregister();
            connection = null;
            network = null;
            networkIdentifier = null;
        }

        protected void UpdateAuto()
        {
            if (!autoNetworkTransfer || connection == null)
            {
                return;
            }

            InputFromNetwork();
            OutputIntoNetwork();
        }
        
        public void InputFromNetwork()
        {
            if (network == null
                || !port.HasFlag(PortType.Input))
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

        public void OutputIntoNetwork()
        {
            if (network == null
                || !port.HasFlag(PortType.Output))
            {
                return;
            }

            extractFilter ??= new InsertableNetworkFilter<T>(ConnectedPort.Container);
            if (Container.TryExtract(extractFilter, out T value))
            {
                network.TryInsert(value);
            }
        }

        public void SetNetwork(PhysicalNetwork<T> network)
        {
            this.network = network;
            network.OnPump += UpdateAuto;
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
            connectedPipe = pipe;
        }

        public void Connect(PhysicalNetworkPort<T> port)
        {
            ConnectedPort = port;
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

        public void Sync(PhysicalNetwork<T> network)
        {
            siblings ??= handler.Ports.Where(p => p.Container == Container && p.port == port);

            siblings.Where(p => p.HasNetwork)
                .ForEach(p => network.Sync(p.network));
        }

        public abstract PipeType AllowedPipeType { get; }
        public abstract void CreateAndSetNetwork(Action<PhysicalNetwork<T>> action);
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false);
        public abstract bool TryInsert(T value, bool simulate = false);

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
