using HarmonyLib;
using Industrica.Patch.Vanilla.Build;
using Nautilus.Handlers;
using System.Collections;
using UnityEngine;

namespace Industrica.Patch.Vanilla
{
    public static class VanillaPatch
    {
        private static bool alreadyPatched = false;

        public static void Patch()
        {
            WaitScreenHandler.RegisterAsyncLoadTask(nameof(VanillaPatch), PatchPrefabs);
        }

        private static IEnumerator PatchPrefabs(WaitScreenHandler.WaitScreenTask task)
        {
            if (alreadyPatched)
            {
                yield break;
            }

            alreadyPatched = true;

            task.Status = Language.main.GetFormat("IndustricaLoading_Patching", nameof(TechType.BaseBioReactor));
            yield return PatchBioReactor.Patch();

            task.Status = Language.main.GetFormat("IndustricaLoading_Patching", nameof(TechType.BaseNuclearReactor));
            yield return PatchNuclearReactor.Patch();

            task.Status = Language.main.GetFormat("IndustricaLoading_Patching", nameof(TechType.Locker));
            yield return PatchLocker.Patch();

            task.Status = Language.main.GetFormat("IndustricaLoading_Patching", nameof(TechType.SmallLocker));
            yield return PatchWallLocker.Patch();
        }
    }
}
