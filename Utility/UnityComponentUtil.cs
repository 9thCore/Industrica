using UnityEngine;

namespace Industrica.Utility
{
    public static class UnityComponentUtil
    {
        public static T WithScale<T>(this T component, float? x = null, float? y = null, float? z = null) where T : Component
        {
            Vector3 scale = component.transform.localScale;
            if (x.HasValue)
            {
                scale.x = x.Value;
            }
            if (y.HasValue)
            {
                scale.y = y.Value;
            }
            if (z.HasValue)
            {
                scale.z = z.Value;
            }

            component.transform.localScale = scale;
            return component;
        }

        public static T WithLocalRotation<T>(this T component, Quaternion rotation) where T : Component
        {
            component.transform.localRotation = rotation;
            return component;
        }

        public static T WithLocalPosition<T>(this T component, Vector3 position) where T : Component
        {
            component.transform.localPosition = position;
            return component;
        }

        public static T WithParent<T>(this T component, Transform parent) where T : Component
        {
            component.transform.SetParent(parent);
            return component;
        }

        public static T GetComponent<T, C>(this T component, out C result) where T : Component where C : Component
        {
            result = component.GetComponent<C>();
            return component;
        }
    }
}
