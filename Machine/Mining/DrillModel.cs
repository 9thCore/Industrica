using UnityEngine;

namespace Industrica.Machine.Mining
{
    public class DrillModel : MonoBehaviour, IBuilderGhostModel
    {
        public void UpdateGhostModelColor(bool allowed, ref Color color)
        {
            if (!allowed)
            {
                return;
            }

            if (!Drill.CheckValidPlacement(transform.position, -transform.up))
            {
                color = Color.yellow;
            }
        }
    }
}
