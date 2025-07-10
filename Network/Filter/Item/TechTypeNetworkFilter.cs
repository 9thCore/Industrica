namespace Industrica.Network.Filter.Item
{
    public class TechTypeNetworkFilter : NetworkFilter<Pickupable>
    {
        public TechType techType;

        public override bool Matches(Pickupable value)
        {
            return value.GetTechType() == techType;
        }
    }
}
