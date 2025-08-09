namespace Industrica.Utility
{
    public class OperationWrapper
    {
        public Type type = Type.Add;

        public int Apply(int a, int b)
        {
            return type switch
            {
                Type.Add => a + b,
                Type.Subtract => a - b,
                Type.Multiply => a * b,
                Type.Divide => b == 0 ? 0 : a / b,
                Type.GreaterThan => a > b ? a : 0,
                Type.LessThan => a < b ? a : 0,
                _ => default,
            };
        }

        public void CycleOperation()
        {
            Set(type switch
            {
                Type.Add => Type.Subtract,
                Type.Subtract => Type.Multiply,
                Type.Multiply => Type.Divide,
                Type.Divide => Type.GreaterThan,
                Type.GreaterThan => Type.LessThan,
                _ => Type.Add
            });
        }

        public void Set(Type type)
        {
            this.type = type;
            OnChange?.Invoke();
        }

        public enum Type
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            GreaterThan,
            LessThan
        }

        public delegate void OperationUpdate();
        public OperationUpdate OnChange;
    }
}
