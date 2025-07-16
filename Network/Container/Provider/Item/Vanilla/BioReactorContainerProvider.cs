namespace Industrica.Network.Container.Provider.Item.Vanilla
{
    public class BioReactorContainerProvider : ItemsContainerProvider
    {
        public override IItemsContainer GetItemsContainer => GetComponent<BaseBioReactor>().container;
    }
}
