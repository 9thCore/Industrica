using Industrica.Network.Filter.Holder;
using Industrica.Register.Equipment;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Storage
{
    public class NetworkFilterStorage : MonoBehaviour, IConstructable
    {
        public Transform root;
        public GenericHandTarget target;
        public FilteredItemsContainer filter;

        private Equipment equipment;

        public NetworkFilterStorage WithStorageRoot(Transform root)
        {
            this.root = root;
            return this;
        }

        public NetworkFilterStorage WithHandTarget(GenericHandTarget target)
        {
            this.target = target;
            target.enabled = false;
            return this;
        }

        public NetworkFilterStorage WithFilteredContainer(FilteredItemsContainer filter)
        {
            this.filter = filter;
            return this;
        }

        public void Start()
        {
            equipment = new Equipment(gameObject, root);
            equipment.SetLabel(nameof(NetworkFilterStorage));
            equipment.isAllowedToAdd = new IsAllowedToAdd(IsAllowedToAdd);
            equipment.compatibleSlotDelegate = new Equipment.DelegateGetCompatibleSlot(GetCompatibleSlot);
            equipment.onAddItem += OnAddItem;
            equipment.onRemoveItem += OnRemoveItem;
            equipment.AddSlot(Slot);
            equipment.Recover(root, Slot);

            target.onHandHover = new();
            target.onHandHover.AddListener(OnHandHover);
            target.onHandClick = new();
            target.onHandClick.AddListener(OnHandClick);
        }

        private void OnRemoveItem(InventoryItem item)
        {
            filter.SetNetworkFilter(null);
        }

        private void OnAddItem(InventoryItem item)
        {
            if (!item.item.TryGetComponent(out NetworkFilterHolder<Pickupable> holder))
            {
                Plugin.Logger.LogError($"Could not get {nameof(NetworkFilterHolder<Pickupable>)}<{nameof(Pickupable)}> in {item.item}, can't apply filter.");
                return;
            }

            filter.SetNetworkFilter(holder.Filter);
        }

        private void OnHandHover(HandTargetEventData data)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand);
            HandReticle.main.SetText(HandReticle.TextType.Hand, "IndustricaEquipment_Filter_Hover", true, GameInput.Button.LeftHand);

            if (equipment.GetItemInSlot(Slot) == null)
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Empty", true);
            }
        }

        private void OnHandClick(HandTargetEventData data)
        {
            Inventory.main.SetUsedStorage(equipment, false);
            Player.main.GetPDA().Open(PDATab.Inventory, transform, null);
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            return pickupable.TryGetComponent(out NetworkFilterHolder<Pickupable> _);
        }

        private bool GetCompatibleSlot(EquipmentType itemType, out string result)
        {
            if (itemType != FilterEquipment.Type
                || equipment.GetItemInSlot(Slot) != null)
            {
                result = default;
                return false;
            }

            result = Slot;
            return true;
        }

        public void OnConstructedChanged(bool constructed)
        {
            target.enabled = constructed;
        }

        public bool IsDeconstructionObstacle()
        {
            return true;
        }

        public bool CanDeconstruct(out string reason)
        {
            if (equipment.GetItemInSlot(Slot) != null)
            {
                reason = "IndustricaEquipment_Filter_ErrorDeconstructStorage".Translate();
                return false;
            }

            reason = default;
            return true;
        }

        public const string Slot = FilterEquipment.CenterSlot;
    }
}
