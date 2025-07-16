namespace Industrica.Network.Container.Provider.Item.Vanilla
{
    public class FiltrationMachineContainerProvider : ItemsContainerProvider
    {
        public override IItemsContainer GetItemsContainer => GetComponent<FiltrationMachine>().storageContainer.container;
    }
}
