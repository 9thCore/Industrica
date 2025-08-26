using Industrica.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace Industrica.Network.Pipe
{
    public abstract class TransferPipe<T> : ConnectionTool<TransferPort<T>> where T : class
    {
        public Transform endCap;

        public abstract PipeType Type { get; }
        public abstract IEnumerator CreatePipe(TransferPort<T> start, TransferPort<T> end);

        public IEnumerator CreatePipe<P>(TechType pipeTechType, TransferPort<T> start, TransferPort<T> end) where P : PlacedTransferPipe<T>
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
            pipe.Connect(start, end);

            UnlinkSegments();
            Reset();
        }

        public override GameObject ParentOfSegmentParent => start.parent.gameObject;
        public override float MaxSegmentLength => MaxPipeLength;

        public override void InitialiseComponent(MultiTool multiTool)
        {
            stretchedPart = UWE.Utils.InstantiateDeactivated(multiTool.stretchedPart).transform.WithParent(transform);
            endCap = UWE.Utils.InstantiateDeactivated(multiTool.endCap).transform.WithParent(transform);
        }

        public override void StartConnection(TransferPort<T> connection)
        {
            connection.LockHover = true;
            start = connection;
            CreateSegment();
            CreateEndCap(connection);
            RestrictPlaceablePorts(connection.port);
            OnConnectionRefresh?.Invoke(this);
        }

        public override void EndConnection(TransferPort<T> connection)
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

        public override bool Available(TransferPort<T> port)
        {
            return port.connectedPort == null;
        }

        public override void Disconnect(TransferPort<T> port)
        {
            port.connectedPort.Disconnect();
            port.Disconnect();
        }

        public GameObject CreateEndCap(TransferPort<T> port)
        {
            GameObject cap = CreateEndCap(segmentParent.transform, endCap.gameObject, port);
            cap.SetActive(false);
            cap.EnsureComponent<SkyApplier>().renderers = cap.GetComponentsInChildren<Renderer>();
            cap.SetActive(true);
            return cap;
        }

        public static GameObject CreateEndCap(Transform parent, GameObject endCapPrefab, TransferPort<T> port)
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
