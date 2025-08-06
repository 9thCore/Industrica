using UnityEngine;

namespace Industrica.Utility
{
    public static class TransformUtil
    {
        public static Transform WithScale(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            Vector3 scale = transform.localScale;
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

            transform.localScale = scale;
            return transform;
        }
    }
}
