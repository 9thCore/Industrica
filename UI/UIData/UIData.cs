using System;
using UnityEngine;

namespace Industrica.UI.UIData
{
    public abstract class UIData : MonoBehaviour
    {
        public abstract void MoveUIData(Vector2 position);
        public abstract void SetAnchor(Vector2 anchor);
    }
}
