using Industrica.Save;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Network.Wire
{
    public class PlacedWire : PlacedConnection
    {
        public static PrefabInfo Info;
        private WirePort start, end;
        private SaveData save;

        public override Vector3 Scale => WireTool.WireScale;
        public override Color StretchedPartColor => WireTool.WireColor;
        public override Color BendColor => WireTool.WireColor;

        public static PrefabInfo Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaPlacedWire", true);

            var prefab = new CustomPrefab(Info);
            var obj = new CloneTemplate(Info, TechType.Pipe);

            obj.ModifyPrefab += go =>
            {
                go.EnsureComponent<DelayedStart>();

                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
                OxygenPipe oxygen = go.GetComponent<OxygenPipe>();
                PlacedWire wire = go.EnsureComponent<PlacedWire>();
                FPModel model = go.GetComponent<FPModel>();

                oxygen.stretchedPart.gameObject.SetActive(false);
                oxygen.stretchedPart.SetParent(go.transform);
                oxygen.endCap.gameObject.SetActive(false);
                oxygen.endCap.SetParent(go.transform);

                wire.stretchedPart = oxygen.stretchedPart;
                WireTool.Setup(wire.stretchedPart);

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

            return Info;
        }

        public void Connect(WirePort start, WirePort end)
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

        protected override void CreateSave()
        {
            save = new SaveData(this);
        }

        protected override void InvalidateSave()
        {
            save.Invalidate();
        }

        protected override void OnDisconnect()
        {
            start.Disconnect();
            end.Disconnect();
            InvalidateSave();
            Destroy(gameObject);
        }

        public void Load(string start, string end, List<Vector3> positions)
        {
            if (!TryFetchPorts(start, end, out WirePort startPort, out WirePort endPort))
            {
                return;
            }

            SetupSegments(positions, endPort.SegmentPosition);
            PrepareRenderers();
            Connect(startPort, endPort);
        }

        public class SaveData : BaseSaveData<SaveData, PlacedWire>
        {
            public SaveData(PlacedWire component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.placedWireData;

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
