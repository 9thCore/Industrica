using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace Industrica.Network.Physical
{
    public abstract class TransferPipe<T> : ConnectionTool<PhysicalNetworkPort<T>> where T : class
    {
        public Transform endCap;

        public abstract PipeType Type { get; }
        public abstract IEnumerator CreatePipe(PhysicalNetworkPort<T> start, PhysicalNetworkPort<T> end);

        public void Setup(OxygenPipe pipe)
        {
            stretchedPart = pipe.stretchedPart;
            craftModel = pipe.craftModel;
            endCap = pipe.endCap;
        }

        public IEnumerator CreatePipe<P>(TechType pipeTechType, PhysicalNetworkPort<T> start, PhysicalNetworkPort<T> end) where P : PlacedTransferPipe<T>
        {
            List<Segment> copy = new(segments);
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(pipeTechType);
            yield return request;

            GameObject result = request.GetResult();
            if (result == null)
            {
                ErrorMessage.AddMessage($"Could not spawn {typeof(T).Name}. Discarding");
                Reset();
                yield break;
            }

            CreateEndCap(end);

            GameObject placedPipe = Instantiate(result);
            placedPipe.transform.position = Vector3.Lerp(start.SegmentPosition, end.SegmentPosition, 0.5f);

            P pipe = placedPipe.GetComponent<P>();
            pipe.SetSegments(segmentParent, copy);
            pipe.ConnectAndCreateNetwork(start, end);

            UnlinkSegments();
            Reset();
        }

        public override GameObject ParentOfSegmentParent => start.parent.gameObject;
        public override float MaxSegmentLength => MaxPipeLength;

        public override void StartConnection(PhysicalNetworkPort<T> connection)
        {
            connection.LockHover = true;
            start = connection;
            CreateSegment();
            CreateEndCap(connection);
            RestrictPlaceablePorts(connection.port);
            OnConnectionRefresh?.Invoke(this);
        }

        public override void EndConnection(PhysicalNetworkPort<T> connection)
        {
            start.LockHover = false;
            start.OnHoverEnd();
            connection.OnHoverEnd();
            CoroutineHost.StartCoroutine(CreatePipe(start, connection));
        }

        public override void Position(Segment segment, Vector3 start)
        {
            Transform aim = Builder.GetAimTransform();
            Vector3 end = GetPlacePosition(aim, out bool skipLowClamp);
            Position(segment, start, end, skipLowClamp);
        }

        public override void Reset()
        {
            if (start != null)
            {
                start.LockHover = false;
                start.OnHoverEnd();
                start = null;
            }

            placeDistance = PlaceDefaultDistance;
            neededPort = PortType.None;
            clearHoldElapsed = 0f;
            hover = null;
            ClearSegments();
            OnConnectionRefresh?.Invoke(this);
        }

        public override bool Available(PhysicalNetworkPort<T> port)
        {
            return !port.HasNetwork;
        }

        public override void Disconnect(PhysicalNetworkPort<T> port)
        {
            port.connectedPort.NetworkDisconnect();
            port.NetworkDisconnect();
        }

        public GameObject CreateEndCap(PhysicalNetworkPort<T> port)
        {
            GameObject cap = CreateEndCap(segmentParent.transform, endCap.gameObject, port);
            cap.SetActive(false);
            cap.EnsureComponent<SkyApplier>().renderers = cap.GetComponentsInChildren<Renderer>();
            cap.SetActive(true);
            return cap;
        }

        public static GameObject CreateEndCap(Transform parent, GameObject endCapPrefab, PhysicalNetworkPort<T> port)
        {
            GameObject endCap = Instantiate(endCapPrefab);
            endCap.transform.SetParent(parent);
            endCap.SetActive(true);

            endCap.transform.position = port.EndCapPosition;
            endCap.transform.rotation = Quaternion.LookRotation(port.transform.up);
            return endCap;
        }

        public static void Position(Segment segment, Vector3 start, Vector3 end, bool skipLowClamp = false)
        {
            Position(segment, start, end, MinPipeLength, MaxPipeLength, skipLowClamp);
        }

        public delegate void ConnectionRefresh(TransferPipe<T> pipe);
        public static ConnectionRefresh OnConnectionRefresh;

        public const float MinPipeLength = 0.5f;
        public const float MaxPipeLength = 2f;
    }
}
