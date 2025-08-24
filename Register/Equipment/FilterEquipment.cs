using Industrica.Utility;
using UnityEngine;

namespace Industrica.Register.Equipment
{
    public static class FilterEquipment
    {
        public static readonly EquipmentType Type = EquipmentUtil.RegisterEquipmentType("IndustricaFilter");
        public const string CenterSlot = "IndustricaFilterSlotCenter";
        public const string LeftCenterSlot = "IndustricaFilterSlotLeft";

        public static void RegisterSlots()
        {
            EquipmentUtil.RegisterEquipmentSlot(CenterSlot, Type);
            EquipmentUtil.RegisterEquipmentSlot(LeftCenterSlot, Type, position: Vector3.right * -102f);
        }
    }
}
