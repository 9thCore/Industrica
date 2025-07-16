using Industrica.Network.Physical;
using Industrica.Network.Systems;
using Industrica.Save;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace Industrica.Item.Network.Placed
{
    public abstract class PlacedTransferPipe<T> : MonoBehaviour
    {

        private PhysicalNetworkPort<T> start, end;
        private GameObject segmentParent;
        private List<TransferPipe.Segment> segments;
        private Transform stretchedPart, endCap;
        private bool disconnectQueued = false;
        private Vector3 lastPlayerPosition = Vector3.zero;

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

        protected abstract void CreateSave();
        protected abstract void InvalidateSave();
        protected abstract void OnObjectDestroySave();

        public void Start()
        {
            stretchedPart = transform.Find("scaleThis");
            endCap = transform.Find("endcap");
            CreateSave();
            UpdateRender();
        }

        public void Update()
        {
            Vector3 playerPosition = Player.main.transform.position;
            if (Vector3.SqrMagnitude(playerPosition - lastPlayerPosition) <= 1f)
            {
                return;
            }
            lastPlayerPosition = playerPosition;

            UpdateRender();
        }

        private void UpdateRender()
        {
            if (segmentParent == null)
            {
                return;
            }

            Vector3 playerPosition = Player.main.transform.position;
            bool flag = Vector3.SqrMagnitude(transform.position - playerPosition) <= MeshUnloadDistanceSquared;
            segmentParent.SetActive(flag);
        }

        public void SetSegments(GameObject segmentParent, List<TransferPipe.Segment> segments)
        {
            this.segmentParent = segmentParent;
            segmentParent.transform.SetParent(transform);
            this.segments = new(segments);
            UpdateRender();
        }

        public void Connect(IPhysicalNetworkPort start, IPhysicalNetworkPort end)
        {
            if (start is not PhysicalNetworkPort<T> startCast
                || end is not PhysicalNetworkPort<T> endCast)
            {
                return;
            }

            this.start = startCast;
            this.end = endCast;

            startCast.Connect(this);
            endCast.Connect(this);

            startCast.Connect(endCast);
            endCast.Connect(startCast);

            if (start.Parent == end.Parent)
            {
                transform.SetParent(start.Parent, true);
            }
        }

        public void ConnectAndCreateNetwork(IPhysicalNetworkPort start, IPhysicalNetworkPort end)
        {
            Connect(start, end);

            if (start.AllowedPipeType == TransferPipe.PipeType.Item)
            {
                CoroutineHost.StartCoroutine(ItemPhysicalNetwork.Create(network =>
                {
                    start.SetNetwork(network);
                    end.SetNetwork(network);
                }));
            }
        }

        public void OnDestroy()
        {
            OnObjectDestroySave();
        }

        public void Disconnect()
        {
            if (disconnectQueued)
            {
                return;
            }

            disconnectQueued = true;
            start.Disconnect();
            end.Disconnect();
            InvalidateSave();
            Destroy(gameObject);
        }

        public TransferPipe.Segment CreateSegment(Vector3 start, Vector3 end)
        {
            TransferPipe.Segment segment = TransferPipe.CreateSegment(segmentParent.transform, stretchedPart.gameObject, segments);
            TransferPipe.Position(segment, start, end);
            return segment;
        }

        public void Load(string start, string end, List<Vector3> positions)
        {
            if (!UniqueIdentifier.TryGetIdentifier(start, out UniqueIdentifier startID)
                || !UniqueIdentifier.TryGetIdentifier(end, out UniqueIdentifier endID))
            {
                Plugin.Logger.LogError($"{this} was incorrectly setup, loaded invalid ids: {start}, {end}. Removing pipe");
                Destroy(gameObject);
                return;
            }

            if (!startID.TryGetComponent(out IPhysicalNetworkPort startPort)
                || !endID.TryGetComponent(out IPhysicalNetworkPort endPort))
            {
                Plugin.Logger.LogError($"{this} was incorrectly setup, loaded ids: {start}, {end}, but they are not ports. Removing pipe");
                Destroy(gameObject);
                return;
            }

            segments = new();
            segmentParent = GameObjectUtil.CreateChild(gameObject, nameof(segmentParent));

            for (int i = 1; i < positions.Count; i++)
            {
                CreateSegment(positions[i - 1], positions[i]);
            }
            CreateSegment(positions.Last(), endPort.PipePosition);

            TransferPipe.CreateEndCap(segmentParent.transform, endCap.gameObject, startPort);
            TransferPipe.CreateEndCap(segmentParent.transform, endCap.gameObject, endPort);

            segmentParent.SetActive(false);
            SkyApplier applier = segmentParent.EnsureComponent<SkyApplier>();
            applier.renderers = segmentParent.GetComponentsInChildren<Renderer>();
            segmentParent.SetActive(true);

            Connect(startPort, endPort);
        }

        public abstract class BaseSaveData<S, C> : ComponentSaveData<S, C> where S : BaseSaveData<S, C> where C : PlacedTransferPipe<T>
        {
            public string startID, endID;
            public List<Vector3> positions = new();
            public BaseSaveData(C component) : base(component) { }

            public override void CopyFromStorage(S data)
            {
                startID = data.startID;
                endID = data.endID;
                positions = data.positions;
            }

            public override void Load()
            {
                Component.Load(startID, endID, positions);
            }

            public override void Save()
            {
                startID = Component.start.Id;
                endID = Component.end.Id;

                positions.Clear();
                Component.segments.ForEach(s => positions.Add(s.Position));
            }
        }

        public const float MeshUnloadDistance = 60f;
        public const float MeshUnloadDistanceSquared = MeshUnloadDistance * MeshUnloadDistance;
    }
}
