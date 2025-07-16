namespace Industrica.Network.Container.Provider.Item
{
    public class NuclearReactorContainerProvider : ItemsContainerProvider
    {
        public override IItemsContainer GetItemsContainer => GetComponent<BaseNuclearReactor>().equipment;
    }
}
