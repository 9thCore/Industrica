using Industrica.UI.UIEvent;
using System;
using TMPro;
using UnityEngine;

namespace Industrica.UI.UIData
{
    public class BackedTextUIData : TextUIData
    {
        public uGUI_Icon background;

        public virtual void SetBackground(uGUI_Icon background)
        {
            this.background = background;
        }

        public override void MoveUIData(Vector2 position)
        {
            base.MoveUIData(position);
            background.transform.localPosition = position;
        }

        public override void SetAnchor(Vector2 anchor)
        {
            base.SetAnchor(anchor);
            
            background.rectTransform.anchorMin = anchor;
            background.rectTransform.anchorMax = anchor;
        }

        public void AddOnClickCallback(Action callback)
        {
            background.gameObject.EnsureComponent<UIClickDetector>().onClick += callback;
        }
    }
}
