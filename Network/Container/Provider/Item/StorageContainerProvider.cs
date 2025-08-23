namespace Industrica.Network.Container.Provider.Item
{
    public class StorageContainerProvider : ItemsContainerProvider
    {
        public override IItemsContainer GetItemsContainer => GetComponentInParent<StorageContainer>().container;
    }
}
