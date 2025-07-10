namespace Industrica.Network.Provider
{
    public interface IContainerProvider<T>
    {
        public T Container { get; }
    }
}
