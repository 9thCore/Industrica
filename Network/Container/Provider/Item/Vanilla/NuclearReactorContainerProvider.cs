namespace Industrica.Network.Container.Provider.Item.Vanilla
{
    public class NuclearReactorContainerProvider : ItemsContainerProvider
    {
        public override IItemsContainer GetItemsContainer => GetComponent<BaseNuclearReactor>().equipment;
    }
}
