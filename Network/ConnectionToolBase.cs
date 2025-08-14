using Industrica.Utility;
using Nautilus.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class ConnectionToolBase : MonoBehaviour
    {
        protected GameObject segmentParent;
        protected float placementTimeout = 0f;
        protected float clearHoldElapsed = 0f;
        protected bool holster = false;
        protected float placeDistance;

        public PortType neededPort = PortType.None;
        public Transform stretchedPart;

        public bool Holstering => holster;
        public bool CanPlace => placementTimeout <= 0f;

        public List<Segment> segments = new(capacity: MaxSegments);

        public abstract bool Placing { get; }
        public abstract bool HoveringOccupiedConnection { get; }
        public abstract string UseText { get; }
        public abstract GameObject ParentOfSegmentParent { get; }
        public abstract void Reset();
        public abstract bool OnLeftHandDown();
        public abstract bool DoesOverrideHand();
        public abstract void InitialiseComponent(MultiTool multiTool);

        protected void RestrictPlaceablePorts(PortType port)
        {
            neededPort = port switch
            {
                PortType.Input => PortType.Output,
                PortType.Output => PortType.Input,
                PortType.InputAndOutput => PortType.None,
                _ => PortType.None
            };
        }

        public void UnlinkSegments()
        {
            segmentParent = null;
        }

        public void ClearSegments()
        {
            if (segmentParent != null)
            {
                Destroy(segmentParent);
                UnlinkSegments();
            }

            segments.Clear();
            placementTimeout = 0f;
        }

        public void ResetPlacementTimeout()
        {
            placementTimeout = PlacementTimeout;
        }

        public void OnEnable()
        {
            Inventory.main.quickSlots.SetIgnoreScrollInput(true);
            holster = false;
            Reset();
        }

        public void OnDisable()
        {
            Inventory.main.quickSlots.SetIgnoreScrollInput(false);
            holster = true;
            Reset();
        }

        public void EnsureSegmentParent()
        {
            if (segmentParent != null)
            {
                return;
            }

            segmentParent = ParentOfSegmentParent.CreateChild(nameof(segmentParent));
        }

        public static Segment CreateSegment(Transform parent, GameObject stretchedPartPrefab, List<Segment> segments, Color stretchedPartColor, Color bendColor, Vector3 scale)
        {
            GameObject segmentRoot = parent.gameObject.CreateChild(nameof(segmentRoot));

            GameObject stretchedPart = Instantiate(stretchedPartPrefab);
            stretchedPart.transform.SetParent(segmentRoot.transform);

            Segment segment = new Segment(segmentRoot, stretchedPart, segments.Count() == 0, stretchedPartColor, bendColor, scale);
            segments.Add(segment);
            return segment;
        }

        public static void Position(Segment segment, Vector3 start, Vector3 end, float minScale, float maxScale, bool skipLowClamp = false)
        {
            segment.Position = start;

            Vector3 offset = end - start;
            Vector3 direction = Vector3.Normalize(offset);
            float distance = skipLowClamp
                ? Mathf.Min(offset.magnitude, maxScale)
                : Mathf.Clamp(offset.magnitude, minScale, maxScale);

            segment.Resize(distance);

            if (direction != Vector3.zero)
            {
                segment.Rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            segment.UpdateEnds();
        }

        public const int MaxSegments = 20;
        public const float PlaceMinDistance = 1f;
        public const float PlaceMaxDistance = 5f;
        public const float PlaceDefaultDistance = 2f;
        public const float PlaceDistanceChange = 16f;
        public const float PlacementTimeout = 0.1f;
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

            public Segment(GameObject segmentRoot, GameObject stretchedPart, bool createExtraBendAtOrigin, Color stretchedPartColor, Color bendColor, Vector3 scale)
            {
                this.segmentRoot = segmentRoot;
                this.stretchedPart = stretchedPart;
                stretchedPart.SetActive(true);
                stretchedPart.transform.localPosition = Vector3.zero;
                stretchedPart.transform.localRotation = Quaternion.identity;
                stretchedPart.transform.localScale = scale;
                Renderer.material.color = stretchedPartColor;

                bend = CreateBend(bendColor, scale);
                if (createExtraBendAtOrigin)
                {
                    _ = CreateBend(bendColor, scale);
                }

                segmentRoot.SetActive(false);
                segmentRoot.EnsureComponent<SkyApplier>().renderers = segmentRoot.GetComponentsInChildren<Renderer>();
                segmentRoot.SetActive(true);
            }

            public void Resize(float length)
            {
                stretchedPart.transform.WithScale(z: length);
                UpdateEnds();
            }

            public void UpdateEnds()
            {
                float distance = stretchedPart.transform.localScale.z;
                bend.transform.localPosition = Vector3.forward * distance;
            }

            private GameObject CreateBend(Color color, Vector3 scale)
            {
                GameObject end = segmentRoot.CreateChild("bend", PrimitiveType.Sphere, scale: scale * 0.0575f);
                Destroy(end.GetComponent<Collider>());
                end.GetComponent<Renderer>().material.color = color;
                MaterialUtils.ApplySNShaders(end, shininess: 6.2f);
                return end;
            }
        }
    }
}
