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

namespace Industrica.Item.Network
{
    public class PlacedTransferPipe : MonoBehaviour
    {
        public static PrefabInfo Info { get; private set; }

        private IPhysicalNetworkPort start, end;
        private GameObject segmentParent;
        private List<TransferPipe.Segment> segments;
        private Transform stretchedPart, endCap;
        private bool disconnectQueued = false;
        private SaveData save;

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaPlacedTransferPipe", true);

            var prefab = new CustomPrefab(Info);
            var obj = new CloneTemplate(Info, TechType.Pipe);

            obj.ModifyPrefab += go =>
            {
                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);
                OxygenPipe oxygen = go.GetComponent<OxygenPipe>();
                PlacedTransferPipe pipe = go.EnsureComponent<PlacedTransferPipe>();
                FPModel model = go.GetComponent<FPModel>();

                oxygen.stretchedPart.gameObject.SetActive(false);
                oxygen.stretchedPart.SetParent(go.transform);
                oxygen.endCap.gameObject.SetActive(false);
                oxygen.endCap.SetParent(go.transform);

                GameObject.DestroyImmediate(oxygen.endCap.GetComponent<Collider>());
                go.DestroyImmediateChildrenWith<Collider>(true);
                GameObject.DestroyImmediate(oxygen.bottomSection.gameObject);
                GameObject.DestroyImmediate(oxygen.craftModel.gameObject);
                GameObject.DestroyImmediate(oxygen.plugOrigin.gameObject);
                GameObject.DestroyImmediate(oxygen);
                GameObject.DestroyImmediate(model.propModel);
                GameObject.DestroyImmediate(model.viewModel);
                GameObject.DestroyImmediate(model);
                GameObject.DestroyImmediate(go.GetComponentInChildren<OxygenArea>());
                GameObject.DestroyImmediate(go.GetComponent<FMOD_CustomLoopingEmitter>());
            };

            prefab.SetGameObject(obj);
            prefab.Register();
        }

        public void Start()
        {
            stretchedPart = transform.Find("scaleThis");
            endCap = transform.Find("endcap");
            save = new(this);
        }

        public void SetSegments(GameObject segmentParent, List<TransferPipe.Segment> segments)
        {
            this.segmentParent = segmentParent;
            segmentParent.transform.SetParent(transform);
            this.segments = new(segments);
        }

        public void Connect(IPhysicalNetworkPort start, IPhysicalNetworkPort end)
        {
            this.start = start;
            this.end = end;

            start.Connect(this);
            end.Connect(this);

            if (start.GameObject.TryGetComponentInParent(out Base seabase)
                && end.GameObject.TryGetComponentInParent(out Base secondSeabase)
                && seabase == secondSeabase)
            {
                Transform parent = segmentParent.transform.parent;

                segmentParent.transform.SetParent(seabase.transform);
                segmentParent.GetComponentsInChildren<SkyApplier>().ForEach(applier =>
                {
                    applier.SetSky(Skies.BaseInterior);
                });
                segmentParent.transform.SetParent(parent);
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
            if (save.Valid)
            {
                save.Save();
            }
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
            save.Invalidate();
            GameObject.Destroy(gameObject);
        }

        public TransferPipe.Segment CreateSegment(Vector3 start, Vector3 end)
        {
            TransferPipe.Segment segment = TransferPipe.CreateSegment(segmentParent.transform, stretchedPart.gameObject, segments);
            TransferPipe.Position(segment, start, end);
            return segment;
        }

        public void Load(SaveData data)
        {
            if (!UniqueIdentifier.TryGetIdentifier(data.startID, out UniqueIdentifier startID)
                || !UniqueIdentifier.TryGetIdentifier(data.endID, out UniqueIdentifier endID))
            {
                Plugin.Logger.LogError($"{this} was incorrectly setup, loaded invalid ids: {data.startID}, {data.endID}. Removing pipe");
                GameObject.Destroy(gameObject);
                return;
            }

            if (!startID.TryGetComponent(out IPhysicalNetworkPort startPort)
                || !endID.TryGetComponent(out IPhysicalNetworkPort endPort))
            {
                Plugin.Logger.LogError($"{this} was incorrectly setup, loaded ids: {data.startID}, {data.endID}, but they are not ports. Removing pipe");
                GameObject.Destroy(gameObject);
                return;
            }

            segments = new();
            segmentParent = GameObjectUtil.CreateChild(gameObject, nameof(segmentParent));

            for (int i = 1; i < data.positions.Count; i++)
            {
                CreateSegment(data.positions[i - 1], data.positions[i]);
            }
            CreateSegment(data.positions.Last(), endPort.PipePosition);

            TransferPipe.CreateEndCap(segmentParent.transform, endCap.gameObject, startPort);
            TransferPipe.CreateEndCap(segmentParent.transform, endCap.gameObject, endPort);

            segmentParent.SetActive(false);
            SkyApplier applier = segmentParent.EnsureComponent<SkyApplier>();
            applier.renderers = segmentParent.GetComponentsInChildren<Renderer>();
            segmentParent.SetActive(true);

            Connect(startPort, endPort);
        }

        public class SaveData : ComponentSaveData<SaveData, PlacedTransferPipe>
        {
            public string startID, endID;
            public List<Vector3> positions = new();
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.placedTransferPipeData;

            public SaveData(PlacedTransferPipe component) : base(component) { }

            public override void Load()
            {
                Component.Load(this);
            }

            public override void Save()
            {
                startID = Component.start.Id;
                endID = Component.end.Id;

                positions.Clear();
                Component.segments.ForEach(s => positions.Add(s.Position));
            }

            public override void CopyFromStorage(SaveData data)
            {
                positions = new(data.positions);
                startID = data.startID;
                endID = data.endID;
            }
        }
    }
}
