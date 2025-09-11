using HarmonyLib;
using Industrica.Utility;
using System.Reflection;

namespace Industrica.Patch.Vanilla.World
{
    [HarmonyPatch(typeof(EntitySlotsPlaceholder), nameof(EntitySlotsPlaceholder.Spawn))]
    public static class EntitySlotsPlaceholderPatch
    {
        private static void Prefix(EntitySlotsPlaceholder __instance)
        {
            EntitySlotData[] data = __instance.slotsData;
            if (data == null)
            {
                return;
            }

            for (int i = data.Length - 1; i >= 0; i--)
            {
                RunSpawners(__instance, data[i]);
            }
        }

        private static void RunSpawners(EntitySlotsPlaceholder placeholder, EntitySlotData slot)
        {
            WorldUtil.RunSpawners(
                slot,
                placeholder.virtualEntityPrefab,
                placeholder.transform,
                slot.localPosition,
                slot.localRotation);
        }
    }
}
