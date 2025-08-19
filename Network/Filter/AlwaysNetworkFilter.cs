namespace Industrica.Network.Filter
{
    public sealed class AlwaysNetworkFilter<T> : NetworkFilter<T>
    {
        public static readonly AlwaysNetworkFilter<T> Instance = new AlwaysNetworkFilter<T>();

        public override bool Matches(T _)
        {
            return true;
        }
    }
}
