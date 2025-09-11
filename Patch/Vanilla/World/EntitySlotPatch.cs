using HarmonyLib;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Patch.Vanilla.World
{
    [HarmonyPatch(typeof(EntitySlot), nameof(EntitySlot.SpawnVirtualEntities))]
    public static class EntitySlotPatch
    {
        public static void Prefix(EntitySlot __instance)
        {
            GameObject virtualEntityPrefab = VirtualEntitiesManager.GetVirtualEntityPrefab();
            virtualEntityPrefab.SetActive(false);

            WorldUtil.RunSpawners(
                __instance,
                virtualEntityPrefab,
                __instance.transform.parent,
                __instance.transform.localPosition,
                __instance.transform.localRotation);
        }
    }
}
