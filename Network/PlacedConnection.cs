using Industrica.Save;
using Industrica.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class PlacedConnection : MonoBehaviour
    {
        public Transform stretchedPart;

        protected GameObject segmentParent;
        protected List<ConnectionToolBase.Segment> segments;
        protected bool disconnectQueued = false;
        protected Vector3 lastPlayerPosition = Vector3.zero;

        public abstract Vector3 Scale { get; }
        public abstract Color StretchedPartColor { get; }
        public abstract Color BendColor { get; }
        protected abstract void CreateSave();
        protected abstract void InvalidateSave();
        protected abstract void OnDisconnect();

        public void Start()
        {
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

        public void SetSegments<S>(GameObject segmentParent, List<S> segments) where S : ConnectionToolBase.Segment
        {
            this.segmentParent = segmentParent;
            segmentParent.transform.SetParent(transform);
            this.segments = new(segments);
            UpdateRender();
        }

        public void Disconnect()
        {
            if (disconnectQueued)
            {
                return;
            }

            disconnectQueued = true;
            OnDisconnect();
        }

        public ConnectionToolBase.Segment CreateSegment(Vector3 start, Vector3 end)
        {
            ConnectionToolBase.Segment segment = ConnectionToolBase.CreateSegment(segmentParent.transform, stretchedPart.gameObject, segments, StretchedPartColor, BendColor, Scale);
            ConnectionToolBase.Position(segment, start, end, float.MinValue, float.MaxValue);
            return segment;
        }

        protected bool TryFetchPorts<T>(string start, string end, out T startPort, out T endPort) where T : Port
        {
            startPort = default;
            endPort = default;

            if (!UniqueIdentifier.TryGetIdentifier(start, out UniqueIdentifier startID)
                || !UniqueIdentifier.TryGetIdentifier(end, out UniqueIdentifier endID))
            {
                Plugin.Logger.LogError($"{this} was incorrectly setup, loaded invalid ids: {start}, {end}. Removing pipe");
                Destroy(gameObject);
                return false;
            }

            if (!startID.TryGetComponent(out startPort)
                || !endID.TryGetComponent(out endPort))
            {
                Plugin.Logger.LogError($"{this} was incorrectly setup, loaded ids: {start}, {end}, but they are not ports. Removing pipe");
                Destroy(gameObject);
                return false;
            }

            return true;
        }

        protected void SetupSegments(List<Vector3> positions, Vector3 lastPosition)
        {
            segments = new();
            segmentParent = gameObject.CreateChild(nameof(segmentParent));

            for (int i = 1; i < positions.Count; i++)
            {
                CreateSegment(positions[i - 1], positions[i]);
            }
            CreateSegment(positions.Last(), lastPosition);
        }

        protected void PrepareRenderers()
        {
            segmentParent.SetActive(false);
            SkyApplier applier = segmentParent.EnsureComponent<SkyApplier>();
            applier.renderers = segmentParent.GetComponentsInChildren<Renderer>();
            segmentParent.SetActive(true);
        }

        public const float MeshUnloadDistance = 60f;
        public const float MeshUnloadDistanceSquared = MeshUnloadDistance * MeshUnloadDistance;

        public abstract class BaseSaveData<S, C> : ComponentSaveData<S, C> where S : BaseSaveData<S, C> where C : PlacedConnection
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

            public override void Save()
            {
                positions.Clear();
                Component.segments.ForEach(s => positions.Add(s.Position));
            }
        }
    }
}
