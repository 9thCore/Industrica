using Industrica.ClassBase;
using Industrica.Item.Filter;
using Industrica.Network.Filter.Holder;
using Industrica.Register.Equipment;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Machine.FilterWriter
{
    public class WriterItemTechTypeFilter : BaseMachine, IRelayPowerChangeListener, IConstructable
    {
        public Transform root;
        public GenericHandTarget handTarget;
        public Workbench workbench;
        private Equipment equipment;

        public WriterItemTechTypeFilter WithStorageRoot(Transform root)
        {
            this.root = root;
            return this;
        }

        public WriterItemTechTypeFilter WithHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public WriterItemTechTypeFilter WithWorkbench(Workbench workbench)
        {
            this.workbench = workbench;
            return this;
        }

        public override void Start()
        {
            base.Start();

            workbench.isValidHandTarget = false;
            workbench.handOverText = "IndustricaWriterItemTechTypeFilter_Access";
            workbench.logic.onItemPickup = new CrafterLogic.OnItemPickup(OnItemPickup);
            workbench.logic.onDone = new CrafterLogic.OnDone(OnDone);
            workbench.logic.onProgress = new CrafterLogic.OnProgress(OnProgress);

            equipment = new Equipment(gameObject, root);
            equipment.SetLabel($"Industrica{nameof(WriterItemTechTypeFilter)}_Storage");
            equipment.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
            equipment.isAllowedToRemove = new IsAllowedToRemove(IsAllowedToRemove);
            equipment.compatibleSlotDelegate = new Equipment.DelegateGetCompatibleSlot(GetCompatibleSlot);
            equipment.onAddItem += OnAddItem;
            equipment.AddSlots(Slots);
            equipment.Recover(root, Slots);

            handTarget.onHandHover = new();
            handTarget.onHandHover.AddListener(OnHandHover);
            handTarget.onHandClick = new();
            handTarget.onHandClick.AddListener(OnHandClick);
        }

        private void OnProgress(float progress)
        {
            if (progress != 0f)
            {
                ConsumeEnergyPerSecond(EnergyUsagePerSecond, out _);
            }

            workbench.OnProgress(progress);
        }

        private void OnAddItem(InventoryItem item)
        {
            if (!powerRelay.IsPowered())
            {
                return;
            }

            StartWrite();
        }

        public void PowerUpEvent(PowerRelay relay)
        {
            StartWrite();
        }

        public void PowerDownEvent(PowerRelay relay)
        {
            if (!Processing())
            {
                return;
            }

            RemoveItem();
            if (workbench.logic != null)
            {
                workbench.logic.ResetCrafter();
            }
            workbench.state = false;
        }

        private void OnItemPickup(GameObject item)
        {
            InventoryItem filter = equipment.GetItemInSlot(FilterSlot);

            if (filter != null
                && filter.item != null
                && filter.item.TryGetComponent(out TechTypeNetworkFilterHolder input)
                && item.TryGetComponent(out TechTypeNetworkFilterHolder holder))
            {
                holder.TechType = input.TechType;
            }

            RemoveFilter();

            workbench.CrafterOnItemPickup(item);
        }

        private void OnDone()
        {
            InventoryItem item = equipment.GetItemInSlot(FilterSlot);
            InventoryItem template = equipment.GetItemInSlot(ItemSlot);
            if (template != null
                && item != null
                && item.item != null
                && item.item.TryGetComponent(out TechTypeNetworkFilterHolder holder))
            {
                holder.TechType = template.techType;
            }

            RemoveItem();

            workbench.CrafterOnDone();
        }

        public void OnHandHover(HandTargetEventData data)
        {
            if (AllSlotsEmpty())
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Empty", true);
            }

            workbench.OnHandHover(null);
        }

        public void OnHandClick(HandTargetEventData data)
        {
            if (workbench.HasCraftedItem())
            {
                workbench.logic.TryPickup();
                return;
            }

            Inventory.main.SetUsedStorage(equipment);
            Player.main.GetPDA().Open(PDATab.Inventory, transform);
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            return TechData.GetEquipmentType(pickupable.GetTechType()) != FilterEquipment.Type
                || pickupable.TryGetComponent(out TechTypeNetworkFilterHolder _);
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            return !Processing();
        }

        private bool Processing()
        {
            return workbench.HasCraftedItem();
        }

        private bool GetCompatibleSlot(EquipmentType itemType, out string result)
        {
            if (itemType == FilterEquipment.Type)
            {
                result = FilterSlot;
                return true;
            } else
            {
                result = ItemSlot;
                return true;
            }
        }

        public void OnConstructedChanged(bool constructed)
        {
            handTarget.enabled = constructed;
        }

        public bool IsDeconstructionObstacle()
        {
            return true;
        }

        public bool CanDeconstruct(out string reason)
        {
            if (!AllSlotsEmpty())
            {
                reason = "IndustricaWriterItemTechTypeFilter_ErrorDeconstruct".Translate();
                return false;
            }

            reason = default;
            return true;
        }

        public static bool IsTechTypeFilter(InventoryItem item)
        {
            return item.item.TryGetComponent(out TechTypeNetworkFilterHolder _);
        }

        public bool FilterSlotEmpty()
        {
            return equipment == null || equipment.GetItemInSlot(FilterSlot) == null;
        }

        public bool ItemSlotEmpty()
        {
            return equipment == null || equipment.GetItemInSlot(ItemSlot) == null;
        }

        public bool AllSlotsEmpty()
        {
            return FilterSlotEmpty() && ItemSlotEmpty();
        }

        private void StartWrite()
        {
            if (Processing()
                || FilterSlotEmpty()
                || ItemSlotEmpty())
            {
                return;
            }

            InventoryItem template = equipment.GetItemInSlot(ItemSlot);

            if (!TechData.GetCraftTime(template.techType, out float seconds))
            {
                seconds = MissingCraftTimeWorkTime;
            }
            seconds += workbench.spawnAnimationDelay + workbench.spawnAnimationDuration;

            TechType filter = ItemTechTypeFilter.Info.TechType;

            if (workbench.logic != null
                && workbench.logic.Craft(filter, seconds))
            {
                workbench.state = true;
                workbench.OnCraftingBegin(filter, seconds);
            }
        }

        private void RemoveFilter()
        {
            RemoveInSlot(FilterSlot);
        }

        private void RemoveItem()
        {
            RemoveInSlot(ItemSlot);
        }

        private void RemoveInSlot(string slot)
        {
            InventoryItem inventoryItem = equipment.RemoveItem(slot, true, false);
            if (inventoryItem != null)
            {
                Pickupable item = inventoryItem.item;
                if (item != null)
                {
                    GameObject.Destroy(item.gameObject);
                }
            }
        }

        public const string FilterSlot = FilterEquipment.LeftCenterSlot;
        public const string ItemSlot = GenericNoFilterEquipment.RightCenterSlot;
        public static readonly string[] Slots = { FilterSlot, ItemSlot };

        // Use the item's craft time as the time it takes to imprint it, or this default if it has no craft time
        // (Supposed to simulate complex items taking a long time to analyse and imprint)
        public const float MissingCraftTimeWorkTime = 3f;

        public const float EnergyUsagePerSecond = 5f;
    }
}
