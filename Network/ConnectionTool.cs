using Industrica.Utility;
using System.Linq;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class ConnectionTool<T> : ConnectionToolBase where T : Port
    {
        protected T start, hover;

        public bool Placing => start != null;
        public bool HoveringAvailableConnection => hover != null && Available(hover);
        public bool HoveringOccupiedConnection => hover != null && !Available(hover);

        public virtual string PlacingLangKey => "ConnectionTool_Place";
        public virtual string DisconnectLangKey => "ConnectionTool_Disconnect";

        public abstract Vector3 Scale { get; }
        public abstract Color StretchedPartColor { get; }
        public abstract Color BendColor { get; }
        public abstract float MaxSegmentLength { get; }
        public abstract bool Available(T port);
        public abstract void StartConnection(T port);
        public abstract void EndConnection(T port);
        public abstract void Position(Segment segment, Vector3 start);
        public abstract void Disconnect(T port);

        public void Connect(T connection)
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

            if (start == null)
            {
                Plugin.Logger.LogError($"Start of connection was removed while connecting, cannot finish pipe");
                Reset();
                return;
            }

            if (!CloseEnoughToLastSegment(connection.SegmentPosition))
            {
                CreateSegment();
                return;
            }

            EndConnection(connection);
        }

        public bool ConnectedTo(T connection)
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
                HandReticle.main.SetText(HandReticle.TextType.Hand, PlacingLangKey, true, GameInput.Button.LeftHand);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, Language.main.GetFormat($"Tooltip_{PlacingLangKey}", segments.Count, MaxSegments), false);
            }

            placementTimeout -= Time.deltaTime;
            UpdateLastSegment();
            UpdateTargettedConnection();

            if (GameInput.GetButtonHeld(Builder.buttonRotateCW))
            {
                placeDistance = Mathf.Clamp(placeDistance + PlaceDistanceChange * Time.deltaTime, PlaceMinDistance, PlaceMaxDistance);
            }
            else if (GameInput.GetButtonHeld(Builder.buttonRotateCCW))
            {
                placeDistance = Mathf.Clamp(placeDistance - PlaceDistanceChange * Time.deltaTime, PlaceMinDistance, PlaceMaxDistance);
            }

            if (HoveringOccupiedConnection && GameInput.GetButtonHeld(GameInput.Button.RightHand))
            {
                clearHoldElapsed += Time.deltaTime;
                HandReticle.main.SetProgress(clearHoldElapsed / ClearHoldTime);

                if (clearHoldElapsed > ClearHoldTime)
                {
                    Disconnect(hover);
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
                || !target.TryGetComponentInParent(out T connection))
            {
                Hover(null);
                return;
            }

            if (!Available(connection))
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

            Vector3 startPos = segments.Select(c => c.EndPosition).DefaultIfEmpty(start.SegmentPosition).Last();

            EnsureSegmentParent();
            Segment segment = CreateSegment(segmentParent.transform, stretchedPart.gameObject, segments, StretchedPartColor, BendColor, Scale);
            Position(segment, startPos);
            ResetPlacementTimeout();
        }

        public Vector3 GetPlacePosition(Transform aim, out bool skipLowClamp)
        {
            if (HoveringAvailableConnection && hover != start && CloseEnoughToLastSegment(hover.SegmentPosition))
            {
                skipLowClamp = true;
                return hover.SegmentPosition;
            }

            if (Physics.Raycast(aim.position, aim.forward, out RaycastHit hit, placeDistance, Builder.placeLayerMask, QueryTriggerInteraction.Ignore))
            {
                skipLowClamp = false;
                return hit.point;
            }

            skipLowClamp = false;
            return aim.position + aim.forward * placeDistance;
        }

        public void Hover(T connection)
        {
            if (hover == connection)
            {
                if (hover != null)
                {
                    hover.OnHover();
                }
                return;
            }

            if (hover != null)
            {
                hover.OnHoverEnd();
            }
            hover = connection;
            if (hover != null)
            {
                hover.OnHoverStart();
            }
        }

        public void OnHoverOccupied()
        {
            if (clearHoldElapsed == 0f)
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand);
            }
            else
            {
                HandReticle.main.UpdateProgress();
                HandReticle.main.SetIcon(HandReticle.IconType.Progress);
            }

            HandReticle.main.SetText(HandReticle.TextType.Hand, DisconnectLangKey, true, GameInput.Button.RightHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, $"Tooltip_{DisconnectLangKey}", true);
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
            return distance <= MaxSegmentLength * MaxSegmentLength;
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
    }
}
