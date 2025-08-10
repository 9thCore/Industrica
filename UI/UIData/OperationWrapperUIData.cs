using Industrica.Operation;

namespace Industrica.UI.UIData
{
    public class OperationWrapperUIData : BackedTextUIData
    {
        private OperationWrapper wrapper;

        public void Setup(OperationWrapper wrapper)
        {
            this.wrapper = wrapper;
            wrapper.OnChange += OnChange;
            OnChange();
        }

        public void OnChange()
        {
            text.text = wrapper.operation.Representation;
        }
    }
}
