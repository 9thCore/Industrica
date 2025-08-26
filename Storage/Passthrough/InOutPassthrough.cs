using Industrica.Network.Container;
using Industrica.Network.Physical;
using Nautilus.Extensions;
using System.Linq;
using UnityEngine;

namespace Industrica.Storage.Passthrough
{
    public class InOutPassthrough<T> : MonoBehaviour where T : class
    {
        private PassthroughContainer<T> container;
        public PassthroughContainer<T> Container => container ??= new PassthroughContainer<T>(Input, Output);

        private PhysicalNetworkPort<T> _output;
        public PhysicalNetworkPort<T> Output => _output.Exists() ?? (_output = GetComponentsInChildren<PhysicalNetworkPort<T>>()
            .Where(p => p.IsOutput)
            .First());

        private PhysicalNetworkPort<T> _input;
        public PhysicalNetworkPort<T> Input => _input.Exists() ?? (_input = GetComponentsInChildren<PhysicalNetworkPort<T>>()
            .Where(p => p.IsInput)
            .First());
    }
}
