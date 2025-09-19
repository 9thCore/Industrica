using UnityEngine;

namespace Industrica.Utility.Smooth
{
    public class FloatSmoothValue : SmoothValue<float>
    {
        public FloatSmoothValue(float initialDuration) : base(initialDuration) { }
        public FloatSmoothValue(float initialValue, float initialDuration) : base(initialValue, initialDuration) { }

        public override float Lerp(float start, float target, float elapsed) => Mathf.Lerp(start, target, elapsed);
    }
}
