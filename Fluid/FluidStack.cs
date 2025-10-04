namespace Industrica.Fluid
{
    public class FluidStack
    {
        public TechType techType;
        public int amount;

        public FluidStack(TechType techType, int amount)
        {
            this.techType = techType;
            this.amount = amount;
        }

        public FluidStack(TechType techType) : this(techType, StackUnit) { }

        public FluidStack() : this(TechType.None, 0) { }

        public const int StackUnit = 1000;
        public static FluidStack Empty => new FluidStack();
    }
}
