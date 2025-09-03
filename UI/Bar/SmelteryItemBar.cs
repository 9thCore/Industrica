using Industrica.Machine.Processing;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.UI.Bar
{
    public class SmelteryItemBar : MonoBehaviour, IBattery
    {
        public Smeltery.SmelteryGroup group;

        public float charge
        {
            get
            {
                if (group == null
                    || group.timeRemaining > group.timeTotal)
                {
                    return 0.01f;
                }

                return group.timeTotal - group.timeRemaining;
            }
            set
            {

            }
        }
        public float capacity => group == null ? 1f : group.timeTotal;

        public string GetChargeValueText()
        {
            return "SmeltMeter_IndustricaSmeltery".Translate((int)(charge / capacity * 100f));
        }
    }
}
