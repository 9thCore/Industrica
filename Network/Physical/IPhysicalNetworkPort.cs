using Industrica.ClassBase;
using Industrica.Item.Network;
using System.ComponentModel;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public interface IPhysicalNetworkPort : IDestroyable
    {
        public TransferPipe.PipeType AllowedPipeType { get; }
        public PortType Port { get; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Vector3 EndCapPosition { get; }
        public Vector3 PipePosition { get; }
        public bool Occupied { get; }
        public string Id { get; }
        public Transform Parent { get; }
        public bool LockHover { set; }
        public void SetNetwork<N>(N network);
        public void Disconnect();
        public bool ShouldBeInteractable(TransferPipe pipe);
        public void OnHoverStart();
        public void OnHover();
        public void OnHoverEnd();
    }
}
