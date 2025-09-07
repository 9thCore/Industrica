using HarmonyLib;
using UnityEngine;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(uGUI_BlueprintEntry), nameof(uGUI_BlueprintEntry.SetIcon))]
    public static class GUIBlueprintEntryPatch
    {
        public static void Postfix(uGUI_BlueprintEntry __instance, TechType techType)
        {
            __instance.gameObject.EnsureComponent<Holder>().techType = techType;
        }

        public class Holder : MonoBehaviour
        {
            public TechType techType;
        }
    }
}
