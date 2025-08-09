using Industrica.Network.BaseModule;
using Industrica.Save;
using Industrica.Utility;
using Unity.Mathematics;
using UnityEngine;

namespace Industrica.Network.Wire
{
    public class WirePort : Port
    {
        public WirePortHandler handler;
        public WirePortRepresentation representation;

        internal int value = WireDefault;
        internal Transform parent;
        private WirePort other;
        private PlacedWire placed;
        private SaveData save;

        public bool Occupied => placed != null;
        public string Id => identifier.Id;
        public override Vector3 SegmentPosition => transform.position;

        public static WirePort CreatePort(GameObject prefab, Vector3 position, Quaternion rotation, PortType type)
        {
            return CreateBasePort<WirePort>(prefab, prefab, position, rotation, type);
        }

        public void Start()
        {
            parent = gameObject.TryGetComponentInParent(out SubRoot seabase) ? seabase.transform : transform.parent;

            if (IsOutput)
            {
                save = new SaveData(this);
            }
        }

        public void Connect(WirePort port)
        {
            other = port;

            if (IsOutput)
            {
                port.SetElectricity(value);
            }
        }

        public void Connect(PlacedWire wire)
        {
            placed = wire;
        }

        public void SetElectricity(int value)
        {
            int clamped = Clamp(value);
            if (this.value == clamped)
            {
                return;
            }

            this.value = clamped;
            OnCharge?.Invoke();
            if (IsOutput && other != null)
            {
                other.SetElectricity(clamped);
            }
        }

        public void Disconnect()
        {
            if (IsInput)
            {
                SetElectricity(WireDefault);
            }

            if (placed != null)
            {
                placed.Disconnect();
                placed = null;
            }

            other = null;
        }

        public void OnDestroy()
        {
            if (IsOutput)
            {
                save.Invalidate();
            }
        }

        public bool ShouldBeInteractable(WireTool tool)
        {
            return !tool.Holstering && CanConnectTo(tool);
        }

        public bool CanConnectTo(WireTool wire)
        {
            return wire.ConnectedTo(this) || port.HasFlag(wire.neededPort);
        }

        public override void CreateRepresentation(GameObject prefab, BaseModuleProvider provider)
        {
            representation = WirePortRepresentation.Create(prefab, this, provider, gameObject);
        }

        public override void EnsureHandlerAndRegister(GameObject prefab, BaseModuleProvider provider)
        {
            handler = prefab.EnsureComponent<WirePortHandler>();
            handler.Register(this);
            handler.WithBaseModule(provider);
        }

        public override string GetClassIDFromHandler()
        {
            return handler.GetClassID();
        }

        public override void OnHover()
        {
            representation.OnHover();
        }

        public override void OnHoverEnd()
        {
            if (lockHover)
            {
                return;
            }

            representation.OnHoverEnd();
        }

        public override void OnHoverStart()
        {
            representation.OnHoverStart();
        }

        public static int Clamp(int value)
        {
            return math.clamp(value, WireMin, WireMax);
        }

        public delegate void ChargeUpdate();
        public event ChargeUpdate OnCharge;

        public const int WireMin = 0;
        public const int WireMax = 99;
        public const int WireDefault = WireMin;

        public class SaveData : ComponentSaveData<SaveData, WirePort>
        {
            public int current;

            public SaveData(WirePort component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.outputWirePortData;

            public override void CopyFromStorage(SaveData data)
            {
                current = data.current;
            }

            public override void Load()
            {
                Component.SetElectricity(current);
            }

            public override void Save()
            {
                current = Component.value;
            }

            public override bool IncludeInSave()
            {
                return current != WireDefault;
            }
        }
    }
}
