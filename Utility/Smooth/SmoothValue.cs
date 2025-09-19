namespace Industrica.Utility.Smooth
{
    public abstract class SmoothValue<T>
    {
        public T value;

        private T start;
        private T target;
        private float duration;
        private float elapsed;
        private bool finished;

        public delegate void OnFinishHandler(T value);
        public event OnFinishHandler OnFinish;

        public abstract T Lerp(T start, T target, float elapsed);

        public SmoothValue(T initialValue, float initialDuration)
        {
            value = initialValue;
            start = initialValue;
            target = initialValue;
            duration = initialDuration;
            elapsed = 0f;
            finished = false;
        }

        public SmoothValue(float initialDuration) : this(default, initialDuration)
        {

        }

        public void Reset()
        {
            elapsed = 0f;
            finished = false;
        }

        public void SetTarget(T target)
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
            value = Lerp(start, target, time);
            elapsed += DayNightCycle.main.deltaTime;
        }
    }
}
