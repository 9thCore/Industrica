using Industrica.Utility;
using System;
using TMPro;

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
            text.text = wrapper.type switch
            {
                OperationWrapper.Type.Add => "+",
                OperationWrapper.Type.Subtract => "-",
                OperationWrapper.Type.Multiply => "*",
                OperationWrapper.Type.Divide => "/",
                OperationWrapper.Type.GreaterThan => ">",
                OperationWrapper.Type.LessThan => "<",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
