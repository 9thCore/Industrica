namespace Industrica.Network.Provider
{
    public interface IContainerProvider<T>
    {
        public IContainer<T> Container { get; }
    }
}
