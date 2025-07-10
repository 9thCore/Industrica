using UnityEngine;

namespace Industrica.Network.Container.Provider
{
    [RequireComponent(typeof(StorageContainer))]
    public class StorageContainerProvider : MonoBehaviour, IContainerProvider<IItemsContainer>
    {
        private StorageContainer storage;
        public StorageContainer Storage
        {
            get
            {
                if (storage == null)
                {
                    storage = GetComponentInParent<StorageContainer>();
                }
                return storage;
            }
        }
        public IItemsContainer Container => Storage.container;
    }
}
