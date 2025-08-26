using Industrica.Storage.Passthrough;

namespace Industrica.Network.Container.Provider.Item
{
    public class InOutPassthroughProvider<T> : ContainerProvider<T> where T : class
    {
        private Container<T> container;
        public override Container<T> Container => container ??= GetComponent<InOutPassthrough<T>>().Container;
    }
}
