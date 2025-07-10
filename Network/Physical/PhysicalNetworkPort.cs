using Industrica.ClassBase;
using Industrica.Item.Network;
using Industrica.Network.Filter;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public abstract class PhysicalNetworkPort<T> : DestroyableMonoBehaviour, IPhysicalNetworkPort
    {
        public PortType port;
        private bool lockHover = false;
        private Transform parent;
        private UniqueIdentifier identifier;
        private PlacedTransferPipe connectedPipe = null;
        private PhysicalPortRepresentation physicalPort = null;

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
        public bool LockHover { set => lockHover = value; }

        protected static Derived CreatePort<Derived>(GameObject root, Vector3 position, Quaternion rotation, PortType type) where Derived : PhysicalNetworkPort<T>
        {
            PhysicalNetworkPortHandler counter = root.EnsureComponent<PhysicalNetworkPortHandler>();

            GameObject portRoot = GameObjectUtil.CreateChild(root, typeof(Derived).Name, position: position, rotation: rotation);

            ChildObjectIdentifier identifier = portRoot.EnsureComponent<ChildObjectIdentifier>();
            identifier.ClassId = counter.GetClassID();

            Derived component = portRoot.EnsureComponent<Derived>();
            component.port = type;

            PhysicalPortRepresentation.CreatePort(portRoot);

            return component;
        }

        public virtual void Start()
        {
            parent = gameObject.TryGetComponentInParent(out Base seabase) ? seabase.transform : transform.parent;
            identifier = gameObject.GetComponent<UniqueIdentifier>();
            physicalPort = gameObject.GetComponentInChildren<PhysicalPortRepresentation>();
        }

        public virtual void Disconnect()
        {
            if (connectedPipe == null)
            {
                return;
            }

            connectedPipe.Disconnect();
            connectedPipe = null;
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
        public abstract bool HasRoomFor(T value);
        public abstract bool TryExtract(NetworkFilter<T> filter, out T value);
        public abstract bool TryInsert(T value);
    }
}
