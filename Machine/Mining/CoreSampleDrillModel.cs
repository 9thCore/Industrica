using UnityEngine;

namespace Industrica.Machine.Mining
{
    public class CoreSampleDrillModel : MonoBehaviour, IBuilderGhostModel
    {
        public void UpdateGhostModelColor(bool allowed, ref Color color)
        {
            if (!allowed)
            {
                return;
            }

            if (!CoreSampleDrill.CheckValidPlacement(transform.position, -transform.up))
            {
                color = Color.yellow;
            }
        }
    }
}
