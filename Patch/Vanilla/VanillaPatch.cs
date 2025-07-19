using HarmonyLib;
using Industrica.Patch.Vanilla.Build;

namespace Industrica.Patch.Vanilla
{
    public static class VanillaPatch
    {
        public static void Patch(Harmony harmony)
        {
            PatchBioReactor.Patch(harmony);
            PatchNuclearReactor.Patch(harmony);
            PatchLocker.Patch();
            PatchWallLocker.Patch();
        }
    }
}
