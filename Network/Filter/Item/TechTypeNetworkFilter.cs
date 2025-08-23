namespace Industrica.Network.Filter.Item
{
    public class TechTypeNetworkFilter : NetworkFilter<Pickupable>
    {
        public TechType techType = TechType.None;

        public override bool Matches(Pickupable value)
        {
            return value.GetTechType() == techType;
        }
    }
}
