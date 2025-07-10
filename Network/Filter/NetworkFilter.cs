namespace Industrica.Network.Filter
{
    public abstract class NetworkFilter<T>
    {
        public abstract bool Matches(T value);
    }
}
