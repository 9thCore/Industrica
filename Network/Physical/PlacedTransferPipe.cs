using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Network.Physical
{
    public abstract class PlacedTransferPipe<T> : PlacedConnection where T : class
    {
        public Transform endCap;

        private PhysicalNetworkPort<T> start, end;

        public static PrefabInfo Register<P>(string classID) where P : PlacedTransferPipe<T>
        {
            PrefabInfo info = PrefabInfo
                .WithTechType(classID, true);

            var prefab = new CustomPrefab(info);
            var obj = new CloneTemplate(info, TechType.Pipe);

            obj.ModifyPrefab += go =>
            {
                PrefabUtils.AddBasicComponents(go, info.ClassID, info.TechType, LargeWorldEntity.CellLevel.Global);
                OxygenPipe oxygen = go.GetComponent<OxygenPipe>();
                P pipe = go.EnsureComponent<P>();
                FPModel model = go.GetComponent<FPModel>();

                oxygen.stretchedPart.gameObject.SetActive(false);
                oxygen.stretchedPart.SetParent(go.transform);
                oxygen.endCap.gameObject.SetActive(false);
                oxygen.endCap.SetParent(go.transform);

                pipe.stretchedPart = oxygen.stretchedPart;
                pipe.endCap = oxygen.endCap;

                DestroyImmediate(oxygen.endCap.GetComponent<Collider>());
                go.DestroyImmediateChildrenWith<Collider>(true);
                DestroyImmediate(oxygen.bottomSection.gameObject);
                DestroyImmediate(oxygen.craftModel.gameObject);
                DestroyImmediate(oxygen.plugOrigin.gameObject);
                DestroyImmediate(oxygen);
                DestroyImmediate(model.propModel);
                DestroyImmediate(model.viewModel);
                DestroyImmediate(model);
                DestroyImmediate(go.GetComponentInChildren<OxygenArea>());
                DestroyImmediate(go.GetComponent<FMOD_CustomLoopingEmitter>());
            };

            prefab.SetGameObject(obj);
            prefab.Register();

            return info;
        }

        public void Connect(PhysicalNetworkPort<T> start, PhysicalNetworkPort<T> end)
        {
            this.start = start;
            this.end = end;

            start.Connect(end);
            end.Connect(start);

            start.Connect(this);
            end.Connect(this);

            if (start.parent == end.parent)
            {
                transform.SetParent(start.parent, true);
            }
        }

        public void ConnectAndCreateNetwork(PhysicalNetworkPort<T> start, PhysicalNetworkPort<T> end)
        {
            Connect(start, end);
            start.CreateAndSetNetwork(end.SetNetwork);
        }

        protected override void OnDisconnect()
        {
            start.NetworkDisconnect();
            end.NetworkDisconnect();
            InvalidateSave();
            Destroy(gameObject);
        }

        public void Load(string start, string end, List<Vector3> positions)
        {
            if (!TryFetchPorts(start, end, out PhysicalNetworkPort<T> startPort, out PhysicalNetworkPort<T> endPort))
            {
                return;
            }

            SetupSegments(positions, endPort.SegmentPosition);
            TransferPipe<T>.CreateEndCap(segmentParent.transform, endCap.gameObject, startPort);
            TransferPipe<T>.CreateEndCap(segmentParent.transform, endCap.gameObject, endPort);
            PrepareRenderers();
            Connect(startPort, endPort);
        }

        public abstract class TransferPipeSaveData<S, C> : BaseSaveData<S, C> where S : TransferPipeSaveData<S, C> where C : PlacedTransferPipe<T>
        {
            public TransferPipeSaveData(C component) : base(component) { }

            public override void Load()
            {
                Component.Load(startID, endID, positions);
            }

            public override void Save()
            {
                base.Save();
                startID = Component.start.Id;
                endID = Component.end.Id;
            }
        }
    }
}
