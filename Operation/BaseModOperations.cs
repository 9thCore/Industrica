namespace Industrica.Operation
{
    public static class BaseModOperations
    {
        public static void Register()
        {
            OperationWrapper.Register<Add>();
            OperationWrapper.Register<Subtract>();
            OperationWrapper.Register<Multiply>();
            OperationWrapper.Register<Divide>();
            OperationWrapper.Register<And>();
            OperationWrapper.Register<Or>();
            OperationWrapper.Register<Xor>();
        }

        public class Add : Operation
        {
            public override int Apply(int a, int b) => a + b;
            public override string Representation => "+";
        }

        public class Subtract : Operation
        {
            public override int Apply(int a, int b) => a - b;
            public override string Representation => "-";
        }

        public class Multiply : Operation
        {
            public override int Apply(int a, int b) => a * b;
            public override string Representation => "*";
        }

        public class Divide : Operation
        {
            public override int Apply(int a, int b) => b == 0 ? 0 : a / b;
            public override string Representation => "/";
        }

        public class And : Operation
        {
            public override int Apply(int a, int b) => a & b;
            public override string Representation => "&";
        }

        public class Or : Operation
        {
            public override int Apply(int a, int b) => a | b;
            public override string Representation => "|";
        }

        public class Xor : Operation
        {
            public override int Apply(int a, int b) => a ^ b;
            public override string Representation => "^";
        }
    }
}
