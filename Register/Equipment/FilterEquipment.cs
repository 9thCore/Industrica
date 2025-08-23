using Industrica.Utility;

namespace Industrica.Register.Equipment
{
    public static class FilterEquipment
    {
        public static readonly EquipmentType Type = EquipmentUtil.RegisterEquipmentType("IndustricaFilter");
        public const string CenterSlot = "IndustricaFilterSlotCenter";

        public static void RegisterSlots()
        {
            EquipmentUtil.RegisterEquipmentSlot(CenterSlot, Type);
        }
    }
}
