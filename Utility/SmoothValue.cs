using System;
using UnityEngine;

namespace Industrica.Utility
{
    public class SmoothValue
    {
        public float value;

        private float start;
        private float target;
        private float duration;
        private float elapsed;
        private bool finished;

        public delegate void OnFinishHandler(float value);
        public event OnFinishHandler OnFinish;

        public SmoothValue(float initialValue, float initialDuration)
        {
            value = initialValue;
            start = initialValue;
            target = initialValue;
            duration = initialDuration;
            elapsed = 0f;
            finished = false;
        }

        public SmoothValue(float initialDuration) : this(0f, initialDuration)
        {

        }

        public void SetTarget(float target)
        {
            this.target = target;
            start = value;
            elapsed = 0f;
            finished = false;
        }

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }

        public bool IsChanging()
        {
            return elapsed < duration;
        }

        public void Update()
        {
            if (elapsed >= duration)
            {
                if (!finished)
                {
                    OnFinish?.Invoke(target);
                    finished = true;
                }

                value = target;
                return;
            }

            float time = elapsed / duration;
            value = Mathf.Lerp(start, target, time);
            elapsed += DayNightCycle.main.deltaTime;
        }
    }
}
