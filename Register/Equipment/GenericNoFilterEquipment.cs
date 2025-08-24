using Industrica.Utility;
using UnityEngine;

namespace Industrica.Register.Equipment
{
    public static class GenericNoFilterEquipment
    {
        public static readonly EquipmentType Type = EquipmentUtil.RegisterEquipmentType("IndustricaGeneric");
        public const string RightCenterSlot = "IndustricaGenericSlotRight";

        public static void RegisterSlots()
        {
            EquipmentUtil.RegisterEquipmentSlot(RightCenterSlot, Type, position: Vector3.right * 102f);
        }
    }
}
