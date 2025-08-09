using UnityEngine;

namespace Industrica.UI
{
    public class ConstructableCanvas : MonoBehaviour, IConstructable
    {
        public Canvas canvas;

        public ConstructableCanvas WithCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            return this;
        }

        public bool CanDeconstruct(out string reason)
        {
            reason = default;
            return true;
        }

        public bool IsDeconstructionObstacle()
        {
            return false;
        }

        public void OnConstructedChanged(bool constructed)
        {
            canvas.gameObject.SetActive(constructed);
        }
    }
}
