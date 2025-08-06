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
    public abstract class PhysicalNetworkPort<T> : Port where T : class
    {
        public bool hasPumpModule = false;

        internal PhysicalNetworkPort<T> connectedPort = null;
        internal Transform parent = null;
        internal PhysicalNetwork<T> network;
        internal UniqueIdentifier networkIdentifier;
        internal PhysicalNetwork<T>.PhysicalConnection connection = null;
        internal PlacedTransferPipe<T> transferPipe = null;

        public abstract Container<T> Container { get; }
        public abstract PipeType AllowedPipeType { get; }
        public abstract void CreateAndSetNetwork(Action<PhysicalNetwork<T>> action);
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false);
        public abstract bool TryInsert(T value, bool simulate = false);
        public abstract PhysicalNetworkPortPump<T> CreatePump();

        public Vector3 EndCapPosition => transform.position + transform.up * 0.06f;
        public override Vector3 SegmentPosition => transform.position + transform.up * 0.2f;
        public string Id => identifier.Id;
        public bool HasNetwork => network != null;

        protected static P CreatePort<P>(GameObject prefab, GameObject root, Vector3 position, Quaternion rotation, PortType type)
            where P : PhysicalNetworkPort<T>
        {
            P component = CreateBasePort<P>(prefab, root, position, rotation, type);
            return component;
        }

        public virtual void Start()
        {
            parent = gameObject.TryGetComponentInParent(out SubRoot seabase) ? seabase.transform : transform.parent;
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
            connection.Deregister();
            connection = null;
            network = null;
            networkIdentifier = null;
        }

        public virtual void SetNetwork(PhysicalNetwork<T> network)
        {
            this.network = network;
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
