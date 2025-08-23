using System.Text;
using UnityEngine;

namespace Industrica.UI.Tooltip
{
    public abstract class AbstractTooltip : MonoBehaviour
    {
        public abstract void Apply(StringBuilder stringBuilder);
    }
}
