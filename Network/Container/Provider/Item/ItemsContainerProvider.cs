namespace Industrica.Network.Container.Provider.Item
{
    public abstract class ItemsContainerProvider : ContainerProvider<Pickupable>
    {
        private Container<Pickupable> container;
        public override Container<Pickupable> Container => container ??= new VanillaItemContainer(GetItemsContainer);
        public abstract IItemsContainer GetItemsContainer { get; }
    }
}
