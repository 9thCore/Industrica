using Industrica.ClassBase;
using Industrica.ClassBase.Addons.Machine;
using UnityEngine;

namespace Industrica.UI
{
    public class PoweredCanvas : BaseMachine, IRelayPowerChangeListener
    {
        public Canvas canvas;

        public PoweredCanvas WithCanvas(Canvas canvas)
        {
            this.canvas = canvas;
            return this;
        }

        public void PowerDownEvent(PowerRelay relay)
        {
            canvas.gameObject.SetActive(false);
        }

        public void PowerUpEvent(PowerRelay relay)
        {
            canvas.gameObject.SetActive(true);
        }
    }
}
