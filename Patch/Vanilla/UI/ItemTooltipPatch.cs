using HarmonyLib;
using Industrica.UI.Tooltip;
using System.Text;
using UnityEngine;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
    [HarmonyBefore("com.snmodding.nautilus")]
    public static class ItemTooltipPatch
    {
        public static void Postfix(StringBuilder sb, GameObject obj)
        {
            obj.GetComponents<AbstractTooltip>().ForEach(tooltip => tooltip.Apply(sb));
        }
    }
}
