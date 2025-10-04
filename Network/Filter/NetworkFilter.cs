namespace Industrica.Network.Filter
{
    public abstract class NetworkFilter<T>
    {
        public int fluidAmount = 0;
        public abstract bool Matches(T value);
    }
}
