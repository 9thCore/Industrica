using Industrica.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace Industrica.Network.Wire
{
    public class WireTool : ConnectionTool<WirePort>
    {
        public override float MaxSegmentLength => MaxWireLength;
        public override GameObject ParentOfSegmentParent => start.parent.gameObject;
        public override string UseText => "Use_IndustricaWire";
        public override Vector3 Scale => WireScale;
        public override Color StretchedPartColor => WireColor;
        public override Color BendColor => WireColor;

        public IEnumerator CreateWire(WirePort start, WirePort end)
        {
            List<Segment> copy = new(segments);
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(PlacedWire.Info.TechType);
            yield return request;

            GameObject result = request.GetResult();
            if (result == null)
            {
                ErrorMessage.AddMessage($"Could not spawn {nameof(WireTool)}. Discarding");
                Reset();
                yield break;
            }

            GameObject placedWire = Instantiate(result);
            placedWire.transform.position = Vector3.Lerp(start.SegmentPosition, end.SegmentPosition, 0.5f);

            PlacedWire wire = placedWire.GetComponent<PlacedWire>();
            wire.SetSegments(segmentParent, copy);
            wire.Connect(start, end);

            UnlinkSegments();
            Reset();
        }

        public override void InitialiseComponent(MultiTool multiTool)
        {
            stretchedPart = UWE.Utils.InstantiateDeactivated(multiTool.stretchedPart).transform.WithParent(transform);
        }

        public override bool Available(WirePort port)
        {
            return !port.Occupied;
        }

        public override void Disconnect(WirePort port)
        {
            port.Disconnect();
        }

        public override void EndConnection(WirePort port)
        {
            start.LockHover = false;
            start.OnHoverEnd();
            port.OnHoverEnd();
            CoroutineHost.StartCoroutine(CreateWire(start, port));
        }

        public override void Position(Segment segment, Vector3 start)
        {
            Transform aim = Builder.GetAimTransform();
            Vector3 end = GetPlacePosition(aim, out bool skipLowClamp);
            Position(segment, start, end, MinWireLength, MaxWireLength, skipLowClamp);
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

        public override void StartConnection(WirePort port)
        {
            port.LockHover = true;
            start = port;
            CreateSegment();
            RestrictPlaceablePorts(port.port);
            OnConnectionRefresh?.Invoke(this);
        }

        public static void Setup(Transform stretchedPart)
        {
            stretchedPart.GetComponentInChildren<Renderer>().material.SetFloat("_SpecInt", 0f);
        }

        public delegate void ConnectionRefresh(WireTool wire);
        public static ConnectionRefresh OnConnectionRefresh;

        public const float MinWireLength = 0.01f;
        public const float MaxWireLength = 2f;

        public static readonly Color WireColor = Color.black;
        public static readonly Vector3 WireScale = Vector3.one * 0.5f;
    }
}
