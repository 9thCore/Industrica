using UnityEngine;

namespace Industrica.Network.Container.Provider.Item
{
    [RequireComponent(typeof(StorageContainer))]
    public class StorageContainerProvider : ItemsContainerProvider
    {
        public override IItemsContainer GetItemsContainer => GetComponentInParent<StorageContainer>().container;
    }
}
