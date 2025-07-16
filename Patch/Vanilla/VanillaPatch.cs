using Industrica.Patch.Vanilla.Build;

namespace Industrica.Patch.Vanilla
{
    public static class VanillaPatch
    {
        public static void Patch()
        {
            PatchLocker.Patch();
            PatchWallLocker.Patch();
            PatchFiltrationMachine.Patch();
        }
    }
}
