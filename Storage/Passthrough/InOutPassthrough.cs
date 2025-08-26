using Industrica.Network.Container;
using Industrica.Network.Pipe;
using Nautilus.Extensions;
using System.Linq;
using UnityEngine;

namespace Industrica.Storage.Passthrough
{
    public class InOutPassthrough<T> : MonoBehaviour where T : class
    {
        private PassthroughContainer<T> container;
        public PassthroughContainer<T> Container => container ??= new PassthroughContainer<T>(Input, Output);

        private TransferPort<T> _output;
        public TransferPort<T> Output => _output.Exists() ?? (_output = GetComponentsInChildren<TransferPort<T>>()
            .Where(p => p.IsOutput)
            .First());

        private TransferPort<T> _input;
        public TransferPort<T> Input => _input.Exists() ?? (_input = GetComponentsInChildren<TransferPort<T>>()
            .Where(p => p.IsInput)
            .First());
    }
}
