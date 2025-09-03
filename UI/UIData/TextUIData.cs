using System;
using TMPro;
using UnityEngine;

namespace Industrica.UI.UIData
{
    public class TextUIData : UIData
    {
        public TextMeshProUGUI text;

        public virtual void SetText(TextMeshProUGUI text)
        {
            SetText(text, HorizontalAlignmentOptions.Center);
        }

        public void SetText(TextMeshProUGUI text, HorizontalAlignmentOptions horizontalAlignment)
        {
            this.text = text;
            text.horizontalAlignment = horizontalAlignment;
        }

        public override void MoveUIData(Vector2 position)
        {
            text.transform.localPosition = position;
        }

        public override void SetAnchor(Vector2 anchor)
        {
            text.rectTransform.anchorMin = anchor;
            text.rectTransform.anchorMax = anchor;
        }

        protected void InvokeUpdate()
        {
            OnUpdate?.Invoke();
        }

        public delegate void DataUpdate();
        public event DataUpdate OnUpdate;
    }
}
