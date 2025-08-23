namespace Industrica.Storage
{
    public abstract class FilteredVanillaItemsContainer : FilteredItemsContainer
    {
        public abstract ItemsContainer GetItemsContainer { get; }

        public override void Start()
        {
            base.Start();
            GetItemsContainer.isAllowedToAdd = IsAllowedToAdd;
        }

        public bool IsAllowedToAdd(Pickupable pickupable, bool verbose) => AllowedToAdd(pickupable);
    }
}
