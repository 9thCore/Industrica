using TMPro;
using UnityEngine;

namespace Industrica.UI.UIData
{
    public class BackedTextUIData : UIData
    {
        public TextMeshProUGUI text;
        public uGUI_Icon background;

        public virtual void SetText(TextMeshProUGUI text)
        {
            this.text = text;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        }

        public virtual void SetBackground(uGUI_Icon background)
        {
            this.background = background;
        }

        public override void MoveUIData(Vector2 position)
        {
            text.transform.localPosition = position;
            background.transform.localPosition = position;
        }

        protected void InvokeUpdate()
        {
            OnUpdate?.Invoke();
        }

        public delegate void DataUpdate();
        public event DataUpdate OnUpdate;
    }
}
