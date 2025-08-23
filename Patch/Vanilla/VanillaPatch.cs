using Industrica.Patch.Vanilla.Build;
using Industrica.Utility;
using Nautilus.Handlers;
using System.Collections;
using UnityEngine;

namespace Industrica.Patch.Vanilla
{
    public static class VanillaPatch
    {
        private static bool alreadyPatched = false;

        public static void Register()
        {
            WaitScreenHandler.RegisterAsyncLoadTask(nameof(VanillaPatch).AsLoadTaskID(), PatchPrefabs);
        }

        private static IEnumerator PatchPrefabs(WaitScreenHandler.WaitScreenTask task)
        {
            if (alreadyPatched)
            {
                yield break;
            }

            alreadyPatched = true;

            task.Status = "IndustricaLoading_Patching".Translate(nameof(TechType.BaseBioReactor));
            yield return PatchBioReactor.Patch();

            task.Status = "IndustricaLoading_Patching".Translate(nameof(TechType.BaseNuclearReactor));
            yield return PatchNuclearReactor.Patch();
        }
    }
}
