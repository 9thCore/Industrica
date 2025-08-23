using UnityEngine;

namespace Industrica.Storage
{
    [RequireComponent(typeof(StorageContainer))]
    public class FilteredStorageContainer : FilteredVanillaItemsContainer
    {
        public override ItemsContainer GetItemsContainer => GetComponent<StorageContainer>().container;
    }
}
