using Industrica.Patch.Vanilla.Build;

namespace Industrica.Patch.Vanilla
{
    public static class VanillaPatch
    {
        public static void Patch()
        {
            PatchBioReactor.Patch();
            PatchNuclearReactor.Patch();
            PatchLocker.Patch();
            PatchWallLocker.Patch();
        }
    }
}
