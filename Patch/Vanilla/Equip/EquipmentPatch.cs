using HarmonyLib;
using Industrica.Register.Equipment;

namespace Industrica.Patch.Vanilla.Equip
{
    [HarmonyPatch(typeof(Equipment), nameof(Equipment.IsCompatible))]
    public static class EquipmentPatch
    {
        public static void Postfix(EquipmentType itemType, EquipmentType slotType, ref bool __result)
        {
            if (slotType == GenericNoFilterEquipment.Type
                && itemType != FilterEquipment.Type)
            {
                __result = true;
            }
        }
    }
}
