namespace Industrica.Operation
{
    public abstract class Operation
    {
        private string id;
        public string Id => id ??= GetType().FullName;

        public abstract int Apply(int a, int b);
        public abstract string Representation { get; }
    }
}
