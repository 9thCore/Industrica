using HarmonyLib;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static VehicleUpgradeConsoleInput;

namespace Industrica.Utility
{
    [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.Awake))]
    public static class EquipmentUtil
    {
        public static EquipmentType RegisterEquipmentType(string name)
        {
            return EnumHandler.AddEntry<EquipmentType>(name);
        }

        public static void RegisterEquipmentSlot(string name, EquipmentType equipmentType, Sprite hint = null, Vector3? position = null, Vector3? scale = null, Action<GameObject> callback = null)
        {
            Equipment.slotMapping[name] = equipmentType;
            CustomSlotData.Add(new(name, equipmentType, hint, position ?? Vector3.zero, scale ?? Vector3.one, callback));
        }

        public static void Recover(this Equipment equipment, Transform root, string slot)
        {
            foreach (Pickupable item in root.GetComponentsInChildren<Pickupable>(true))
            {
                if (equipment.GetItemInSlot(slot) != null)
                {
                    Plugin.Logger.LogWarning($"[{equipment._label}] Found extra item in {slot} ({item.gameObject}), destroying...");
                    GameObject.Destroy(item.gameObject);
                    continue;
                }

                if (!equipment.AddItem(slot, new InventoryItem(item)))
                {
                    Plugin.Logger.LogWarning($"[{equipment._label}] Found invalid item in {slot} ({item.gameObject}), destroying...");
                    GameObject.Destroy(item.gameObject);
                }
            }
        }

        private static void Prefix(uGUI_Equipment __instance)
        {
            CreateSlots(__instance.GetComponentInChildren<uGUI_EquipmentSlot>(true));
        }

        private static void CreateSlots(uGUI_EquipmentSlot slot)
        {
            if (slot == null)
            {
                Plugin.Logger.LogError($"Found no {nameof(uGUI_EquipmentSlot)} in tree of {nameof(uGUI_Equipment)}?? Cannot add custom equipment slots.");
            }

            foreach (SlotData data in CustomSlotData)
            {
                GameObject instance = UWE.Utils.InstantiateDeactivated(
                    slot.gameObject,
                    slot.transform.parent,
                    data.Position,
                    Quaternion.identity,
                    data.Scale);

                instance.name = data.Name;

                uGUI_EquipmentSlot instanceSlot = instance.GetComponent<uGUI_EquipmentSlot>();
                instanceSlot.slot = data.Name;

                Image component = instanceSlot.hint.GetComponent<Image>();
                if (data.Hint != null)
                {
                    component.sprite = data.Hint;
                } else
                {
                    component.enabled = false;
                }

                data.Callback?.Invoke(instance);
            }
        }

        private static readonly List<SlotData> CustomSlotData = new();
        private record SlotData(string Name, EquipmentType EquipmentType, Sprite Hint, Vector3 Position, Vector3 Scale, Action<GameObject> Callback);
    }
}
