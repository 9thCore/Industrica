using Industrica.Network.BaseModule;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Network
{
    public abstract class Port : MonoBehaviour
    {
        protected bool lockHover = false;

        public UniqueIdentifier identifier;
        public PortType port;

        public bool IsInput => port == PortType.Input;
        public bool IsOutput => port == PortType.Output;
        public abstract Vector3 SegmentPosition { get; }
        public bool LockHover { set => lockHover = value; }

        protected static P CreateBasePort<P>(GameObject prefab, GameObject root, Vector3 position, Quaternion rotation, PortType type)
            where P : Port
        {
            GameObject portRoot = root.CreateChild(typeof(P).Name, position: position, rotation: rotation);

            BaseModuleProvider provider = prefab.GetComponent<BaseModuleProvider>();
            PortIDAssigner assigner = prefab.EnsureComponent<PortIDAssigner>();

            ChildObjectIdentifier identifier = portRoot.EnsureComponent<ChildObjectIdentifier>();

            P component = portRoot.EnsureComponent<P>();
            component.port = type;
            component.identifier = identifier;
            component.EnsureHandlerAndRegister(prefab, provider);
            component.CreateRepresentation(prefab, provider);

            identifier.ClassId = assigner.GetClassIDAndCycle();
            return component;
        }

        public abstract void EnsureHandlerAndRegister(GameObject prefab, BaseModuleProvider provider);
        public abstract void CreateRepresentation(GameObject prefab, BaseModuleProvider provider);
        public abstract void OnHoverStart();
        public abstract void OnHover();
        public abstract void OnHoverEnd();
    }
}
