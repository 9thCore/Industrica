using Industrica.Utility;
using Nautilus.Utility;
using System;
using UnityEngine;

namespace Industrica.ClassBase.Modules.ProcessingMachine
{
    public class ProcessingMachineItemModule : ProcessingMachineModule
    {
        public bool validHandTarget;
        public Vector2 uiPosition = default;
        public StorageContainer storageContainer;
        public ItemsContainer ItemsContainer => storageContainer.container;

        public ProcessingMachineItemModule WithStorageContainer(StorageContainer storageContainer, bool validHandTarget)
        {
            this.storageContainer = storageContainer;
            this.validHandTarget = validHandTarget;
            return this;
        }

        public ProcessingMachineItemModule WithStorageContainer(StorageContainer storageContainer, bool validHandTarget, Vector2 uiPosition)
        {
            this.storageContainer = storageContainer;
            this.validHandTarget = validHandTarget;
            this.uiPosition = uiPosition;
            return this;
        }

        public override void OnStart()
        {
            storageContainer.isValidHandTarget = validHandTarget;

            if (!interaction.HasFlag(Interaction.AllowInput))
            {
                ItemsContainer.isAllowedToAdd = SNUtil.DisallowAction;
            }

            if (!interaction.HasFlag(Interaction.AllowOutput))
            {
                ItemsContainer.isAllowedToRemove = SNUtil.DisallowAction;
            }

            if (interaction.HasFlag(Interaction.LookupRecipeOnInput))
            {
                ItemsContainer.onAddItem += RequestRecipeLookup;
            }
        }

        public override void Use(bool append, int index)
        {
            if (interaction.HasFlag(Interaction.DisableUse))
            {
                return;
            }

            Plugin.Logger.LogWarning($"Adding {index} to used storage.");
            Inventory.main.SetUsedStorage(ItemsContainer, append);

            if (uiPosition != default)
            {
                Plugin.Logger.LogWarning($"Moving {index} to {uiPosition}.");
                UICustomContainerHandler.MoveContainerUI(UICustomContainerHandler.Tab.torpedoStorage[index], uiPosition);
            }
        }

        public override bool IsEmpty()
        {
            return ItemsContainer.IsEmpty();
        }

        private void RequestRecipeLookup(InventoryItem _)
        {
            holder.QueueRecipeCheck();
        }
    }
}
