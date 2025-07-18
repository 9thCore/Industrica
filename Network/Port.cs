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
            GameObject portRoot = GameObjectUtil.CreateChild(root, typeof(P).Name, position: position, rotation: rotation);

            ChildObjectIdentifier identifier = portRoot.EnsureComponent<ChildObjectIdentifier>();

            P component = portRoot.EnsureComponent<P>();
            component.port = type;
            component.identifier = identifier;
            component.EnsureHandlerAndRegister(prefab);
            component.CreateRepresentation(prefab);

            identifier.ClassId = component.GetClassIDFromHandler();
            return component;
        }

        public abstract string GetClassIDFromHandler();
        public abstract void EnsureHandlerAndRegister(GameObject prefab);
        public abstract void CreateRepresentation(GameObject prefab);
        public abstract void OnHoverStart();
        public abstract void OnHover();
        public abstract void OnHoverEnd();
    }
}
