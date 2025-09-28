using Industrica.Network.Container;
using Industrica.Network.Filter;

using Industrica.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Network.Pipe
{
    public abstract class TransferPort<T> : Port where T : class
    {
        internal TransferPort<T> connectedPort = null;
        internal Transform parent = null;
        internal PlacedTransferPipe<T> transferPipe = null;
        private readonly List<ISubscriber> subscribers = new();

        public abstract Container<T> Container { get; }
        public abstract PipeType AllowedPipeType { get; }
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false);
        public abstract bool TryInsert(T value, bool simulate = false);

        public Vector3 EndCapPosition => transform.position + transform.up * 0.06f;
        public override Vector3 SegmentPosition => transform.position + transform.up * 0.2f;
        public string Id => identifier.Id;

        protected static P CreatePort<P>(GameObject prefab, GameObject root, Vector3 position, Quaternion rotation, PortType type, bool outside)
            where P : TransferPort<T>
        {
            P component = CreateBasePort<P>(prefab, root, position, rotation, type, outside);
            return component;
        }

        public virtual void Start()
        {
            parent = gameObject.TryGetComponentInParent(out SubRoot seabase) ? seabase.transform : transform.parent;
        }

        public void OnDestroy()
        {
            Disconnect();
        }

        public virtual void Disconnect()
        {
            if (transferPipe == null)
            {
                return;
            }

            PlacedTransferPipe<T> copy = transferPipe;
            transferPipe = null;
            copy.Disconnect();
            connectedPort = null;

            for (int i = 0; i < subscribers.Count; i++)
            {
                subscribers[i].OnDisconnect();
            }
        }

        public void Connect(PlacedTransferPipe<T> pipe)
        {
            transferPipe = pipe;
        }

        public void Connect(TransferPort<T> port)
        {
            connectedPort = port;
            
            for (int i = 0; i < subscribers.Count; i++)
            {
                subscribers[i].OnConnect();
            }
        }

        public bool ShouldBeInteractable(TransferPipe<T> pipe)
        {
            return !pipe.Holstering && AllowedPipeType == pipe.Type && CanConnectTo(pipe);
        }

        public bool CanConnectTo(TransferPipe<T> pipe)
        {
            return pipe.ConnectedTo(this) || port.HasFlag(pipe.neededPort);
        }

        public void RegisterSubscriber(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void UnregisterSubscriber(ISubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public interface ISubscriber
        {
            public void OnConnect();
            public void OnDisconnect();
        }
    }
}
