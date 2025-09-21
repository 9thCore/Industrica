using UnityEngine;

namespace Industrica.UI.Inventory.Custom
{
    public abstract class DisplayInfo
    {
        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                if (position == value)
                {
                    return;
                }

                OnSetPosition(value);
                position = value;
            }
        }


        private Vector2 scale = Vector2.one;
        public Vector2 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (scale == value)
                {
                    return;
                }

                OnSetScale(value);
                scale = value;
            }
        }

        private float rotation;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                if (rotation == value)
                {
                    return;
                }

                OnSetRotation(value);
                rotation = value;
            }
        }

        public void Hide()
        {
            if (UICustomDisplayHandler.Instance == null)
            {
                return;
            }

            UICustomDisplayHandler.Instance.Hide(this);
        }

        public abstract void OnSetPosition(Vector2 position);
        public abstract void OnSetScale(Vector2 scale);
        public abstract void OnSetRotation(float rotation);
        public abstract void Show();
        public abstract void OnCreate();
        public abstract void Update();
        public abstract void SetActive(bool active);
    }
}
