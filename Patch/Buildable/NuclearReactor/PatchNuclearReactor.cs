using HarmonyLib;

namespace Industrica.Patch.Buildable.NuclearReactor
{
    [HarmonyPatch]
    public static class PatchNuclearReactor
    {
        [HarmonyPatch(typeof(BaseNuclearReactor), nameof(BaseNuclearReactor.Start))]
        [HarmonyPostfix]
        private static void Patch(BaseNuclearReactor __instance)
        {
            __instance.gameObject.EnsureComponent<NuclearReactorSteamer>();
        }
    }
}
