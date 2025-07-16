using Industrica.Item.Network.Placed;
using Industrica.Network;
using Industrica.Network.Physical;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace Industrica.Item.Network
{
    public class TransferPipe : PlayerTool
    {
        private IPhysicalNetworkPort start;
        private IPhysicalNetworkPort hover;
        private GameObject segmentParent;
        private float placementTimeout = 0f;
        private float clearHoldElapsed = 0f;
        private PortType neededPort = PortType.None;
        [SerializeField]
        private Transform stretchedPart, endCap;
        [SerializeField]
        private GameObject craftModel;
        private bool holster = false;
        private float placeDistance;

        public PortType NeededPort => neededPort;
        public bool Placing => start.IsAlive();
        public bool Holstering => holster;
        public bool HoveringAvailableConnection => hover.IsAlive() && !hover.Occupied;
        public bool HoveringOccupiedConnection => hover.IsAlive() && hover.Occupied;
        public bool CanPlace => placementTimeout <= 0f;

        public PipeType type = PipeType.None;
        public List<Segment> segments = new(capacity: MaxSegments);

        public void Setup(OxygenPipe pipe)
        {
            stretchedPart = pipe.stretchedPart;
            endCap = pipe.endCap;
            craftModel = pipe.craftModel;
        }

        public System.Collections.IEnumerator CreatePipe<T, S>(TechType pipeTechType, IPhysicalNetworkPort start, IPhysicalNetworkPort end) where T : PlacedTransferPipe<S>
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

            GameObject placedPipe = GameObject.Instantiate(result);
            placedPipe.transform.position = Vector3.Lerp(start.PipePosition, end.PipePosition, 0.5f);

            T pipe = placedPipe.GetComponent<T>();
            pipe.SetSegments(segmentParent, copy);
            pipe.ConnectAndCreateNetwork(start, end);

            UnlinkSegments();
            Reset();
        }

        public System.Collections.IEnumerator CreateItemPipe(IPhysicalNetworkPort start, IPhysicalNetworkPort end)
        {
            yield return CreatePipe<PlacedItemTransferPipe, Pickupable>(PlacedItemTransferPipe.Info.TechType, start, end);
        }

        public void StartConnection(IPhysicalNetworkPort connection)
        {
            connection.LockHover = true;
            start = connection;
            CreateSegment();

            neededPort = connection.Port switch
            {
                PortType.Input => PortType.Output,
                PortType.Output => PortType.Input,
                PortType.InputAndOutput => PortType.InputAndOutput,
                _ => PortType.None
            };

            OnConnectionRefresh?.Invoke(this);

            CreateEndCap(connection);
        }

        public void EndConnection(IPhysicalNetworkPort connection)
        {
            if (start.IsDestroyed())
            {
                Plugin.Logger.LogError($"Start of connection was removed while connecting, cannot finish pipe");
                Reset();
                return;
            }

            if (!CloseEnoughToLastSegment(connection.PipePosition))
            {
                CreateSegment();
                return;
            }

            start.LockHover = false;
            start.OnHoverEnd();
            connection.OnHoverEnd();
            if (start.AllowedPipeType == PipeType.Item)
            {
                CoroutineHost.StartCoroutine(CreateItemPipe(start, connection));
            }
        }

        public void Connect(IPhysicalNetworkPort connection)
        {
            if (!Placing)
            {
                StartConnection(connection);
                return;
            }

            if (connection == start)
            {
                return;
            }

            EndConnection(connection);
        }

        public bool ConnectedTo(IPhysicalNetworkPort connection)
        {
            return start == connection;
        }

        public void Update()
        {
            if (usingPlayer != null)
            {
                UpdateTool();
                return;
            }
        }

        public void UpdateTool()
        {
            if (segments.Count > 0)
            {
                HandReticle.main.SetText(HandReticle.TextType.Hand, "IndustricaPipe_Place", true, GameInput.Button.LeftHand);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, Language.main.GetFormat("IndustricaPipe_Place_Tooltip", segments.Count, MaxSegments), false);
            }

            placementTimeout -= Time.deltaTime;
            UpdateLastSegment();
            UpdateTargettedConnection();

            if (GameInput.GetButtonHeld(Builder.buttonRotateCW))
            {
                placeDistance = Mathf.Clamp(placeDistance + PlaceDistanceChange * Time.deltaTime, PlaceMinDistance, PlaceMaxDistance);
            } else if (GameInput.GetButtonHeld(Builder.buttonRotateCCW))
            {
                placeDistance = Mathf.Clamp(placeDistance - PlaceDistanceChange * Time.deltaTime, PlaceMinDistance, PlaceMaxDistance);
            }

            if (HoveringOccupiedConnection && GameInput.GetButtonHeld(GameInput.Button.RightHand))
            {
                clearHoldElapsed += Time.deltaTime;
                HandReticle.main.SetProgress(clearHoldElapsed / ClearHoldTime);

                if (clearHoldElapsed > ClearHoldTime)
                {
                    hover.Disconnect();
                    clearHoldElapsed = 0f;
                }
            }
            else
            {
                clearHoldElapsed = 0f;
            }
        }

        public void UpdateTargettedConnection()
        {
            if (!TryGetTarget(out GameObject target)
                || !target.TryGetComponentInParent(out IPhysicalNetworkPort connection))
            {
                Hover(null);
                return;
            }

            if (connection.Occupied)
            {
                Hover(connection);
                OnHoverOccupied();
                return;
            }

            Hover(connection);
        }

        public bool TryGetTarget(out GameObject target)
        {
            if (DoesOverrideHand())
            {
                return Targeting.GetTarget(Player.main.gameObject, 2f, out target, out _);
            }

            target = Player.main.guiHand.GetActiveTarget();
            return target != null;
        }

        public void UpdateLastSegment()
        {
            if (segments.Count == 0)
            {
                return;
            }

            Segment segment = segments.Last();
            Position(segment, segment.Position);
        }

        public void CreateSegment()
        {
            if (!CanPlace || segments.Count >= MaxSegments)
            {
                return;
            }

            Vector3 startPos = segments.Select(c => c.EndPosition).DefaultIfEmpty(start.PipePosition).Last();

            EnsureSegmentParent();
            Segment segment = CreateSegment(segmentParent.transform, stretchedPart.gameObject, segments);
            Position(segment, startPos);
            ResetPlacementTimeout();
        }

        public static Segment CreateSegment(Transform parent, GameObject stretchedPartPrefab, List<Segment> segments)
        {
            GameObject segmentRoot = GameObjectUtil.CreateChild(parent.gameObject, nameof(segmentRoot));

            GameObject stretchedPart = GameObject.Instantiate(stretchedPartPrefab);
            stretchedPart.transform.SetParent(segmentRoot.transform);

            Segment segment = new Segment(segmentRoot, stretchedPart, segments.Count() == 0);
            segments.Add(segment);
            return segment;
        }

        public GameObject CreateEndCap(IPhysicalNetworkPort port)
        {
            GameObject cap = CreateEndCap(segmentParent.transform, endCap.gameObject, port);
            cap.SetActive(false);
            cap.EnsureComponent<SkyApplier>().renderers = cap.GetComponentsInChildren<Renderer>();
            cap.SetActive(true);
            return cap;
        }

        public static GameObject CreateEndCap(Transform parent, GameObject endCapPrefab, IPhysicalNetworkPort port)
        {
            GameObject endCap = GameObject.Instantiate(endCapPrefab);
            endCap.transform.SetParent(parent);
            endCap.SetActive(true);

            endCap.transform.position = port.EndCapPosition;
            endCap.transform.rotation = Quaternion.LookRotation(port.Transform.up);
            return endCap;
        }

        public void EnsureSegmentParent()
        {
            if (start.IsDestroyed()
                || segmentParent != null)
            {
                return;
            }

            segmentParent = GameObjectUtil.CreateChild(start.Parent.gameObject, nameof(segmentParent));
        }

        public Vector3 GetPlacePosition(Transform aim, out bool skipLowClamp)
        {
            if (HoveringAvailableConnection && hover != start && CloseEnoughToLastSegment(hover.PipePosition))
            {
                skipLowClamp = true;
                return hover.PipePosition;
            }

            if (Physics.Raycast(aim.position, aim.forward, out RaycastHit hit, placeDistance, Builder.placeLayerMask, QueryTriggerInteraction.Ignore))
            {
                skipLowClamp = false;
                return hit.point;
            }

            skipLowClamp = false;
            return aim.position + aim.forward * placeDistance;
        }

        public void Position(Segment segment, Vector3 start)
        {
            Transform aim = Builder.GetAimTransform();
            Vector3 end = GetPlacePosition(aim, out bool skipLowClamp);
            Position(segment, start, end, skipLowClamp);
        }

        public static void Position(Segment segment, Vector3 start, Vector3 end, bool skipLowClamp = false)
        {
            segment.Position = start;

            Vector3 offset = end - start;
            Vector3 direction = Vector3.Normalize(offset);
            float distance = skipLowClamp
                ? Mathf.Min(offset.magnitude, MaxPipeScale)
                : Mathf.Clamp(offset.magnitude, MinPipeScale, MaxPipeScale);

            segment.Resize(distance);

            if (direction == Vector3.zero)
            {
                segment.UpdateEnds();
                return;
            }

            segment.Rotation = Quaternion.LookRotation(direction, Vector3.up);
            segment.UpdateEnds();
        }

        public bool CloseEnoughToLastSegment(Vector3 target)
        {
            if (segments.Count == 0)
            {
                return false;
            }

            return CloseEnoughToConnectSegment(segments.Last(), target);
        }

        public bool CloseEnoughToConnectSegment(Segment segment, Vector3 target)
        {
            float distance = Vector3.SqrMagnitude(segment.Position - target);
            return distance <= MaxPipeScale * MaxPipeScale;
        }

        public void UnlinkSegments()
        {
            segmentParent = null;
        }

        public void ClearSegments()
        {
            if (segmentParent != null)
            {
                GameObject.Destroy(segmentParent);
                UnlinkSegments();
            }
            
            segments.Clear();
            placementTimeout = 0f;
        }

        public void ResetPlacementTimeout()
        {
            placementTimeout = PlacementTimeout;
        }

        public void Hover(IPhysicalNetworkPort connection)
        {
            if (hover == connection)
            {
                if (hover.IsAlive())
                {
                    hover.OnHover();
                }
                return;
            }

            if (hover.IsAlive())
            {
                hover.OnHoverEnd();
            }
            hover = connection;
            if (hover.IsAlive())
            {
                hover.OnHoverStart();
            }
        }

        public void OnHoverOccupied()
        {
            if (clearHoldElapsed == 0f)
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand);
            } else
            {
                HandReticle.main.UpdateProgress();
                HandReticle.main.SetIcon(HandReticle.IconType.Progress);
            }

            HandReticle.main.SetText(HandReticle.TextType.Hand, "IndustricaPipe_Disconnect", true, GameInput.Button.RightHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "IndustricaPipe_Disconnect_Tooltip", true);
        }

        public void Reset()
        {
            if (start.IsAlive())
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

        public override void OnDraw(Player p)
        {
            Inventory.main.quickSlots.SetIgnoreScrollInput(true);
            holster = false;
            Reset();
            base.OnDraw(p);
        }

        public override void OnHolster()
        {
            Inventory.main.quickSlots.SetIgnoreScrollInput(false);
            holster = true;
            Reset();
            base.OnHolster();
        }

        public override bool DoesOverrideHand()
        {
            return HoveringAvailableConnection || Placing;
        }

        public override bool OnLeftHandDown()
        {
            if (HoveringAvailableConnection)
            {
                Connect(hover);
                return true;
            }

            if (Placing)
            {
                CreateSegment();
                return true;
            }

            return false;
        }

        public enum PipeType
        {
            None,
            Item
        }

        public delegate void ConnectionRefresh(TransferPipe pipe);
        public static ConnectionRefresh OnConnectionRefresh;

        public const int MaxSegments = 20;
        public const float PlaceMinDistance = 1f;
        public const float PlaceMaxDistance = 5f;
        public const float PlaceDefaultDistance = 2f;
        public const float PlaceDistanceChange = 16f;
        public const float PlacementTimeout = 0.1f;
        public const float MinPipeScale = 0.5f;
        public const float MaxPipeScale = 2f;
        public const float ClearHoldTime = 0.75f;

        public class Segment
        {
            public Vector3 Position
            {
                get => segmentRoot.transform.position;
                set => segmentRoot.transform.position = value;
            }

            public Quaternion Rotation
            {
                get => segmentRoot.transform.rotation;
                set => segmentRoot.transform.rotation = value;
            }
            public Vector3 EndPosition => stretchedPart.transform.position + stretchedPart.transform.forward * stretchedPart.transform.localScale.z;
            public Renderer Renderer => segmentRoot.GetComponentInChildren<Renderer>();

            private readonly GameObject segmentRoot;
            private readonly GameObject stretchedPart;
            private readonly GameObject bend;

            public Segment(GameObject segmentRoot, GameObject stretchedPart, bool createExtraBendAtOrigin)
            {
                this.segmentRoot = segmentRoot;
                this.stretchedPart = stretchedPart;
                stretchedPart.SetActive(true);
                stretchedPart.transform.localPosition = Vector3.zero;
                stretchedPart.transform.localRotation = Quaternion.identity;

                bend = CreateBend();
                if (createExtraBendAtOrigin)
                {
                    _ = CreateBend();
                }

                segmentRoot.SetActive(false);
                segmentRoot.EnsureComponent<SkyApplier>().renderers = segmentRoot.GetComponentsInChildren<Renderer>();
                segmentRoot.SetActive(true);
            }

            public void Resize(float length)
            {
                GameObjectUtil.Resize(stretchedPart.transform, z: length);
                UpdateEnds();
            }

            public void UpdateEnds()
            {
                float distance = stretchedPart.transform.localScale.z;
                bend.transform.localPosition = Vector3.forward * distance;
            }

            private GameObject CreateBend()
            {
                GameObject end = GameObjectUtil.CreateChild(segmentRoot, "bend", PrimitiveType.Sphere, scale: Vector3.one * 0.0575f);
                GameObject.Destroy(end.GetComponent<Collider>());
                end.GetComponent<Renderer>().material.color = new Color32(225, 224, 222, 255);
                MaterialUtils.ApplySNShaders(end, shininess: 6.2f);
                return end;
            }
        }
    }
}
