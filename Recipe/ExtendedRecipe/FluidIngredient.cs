using Industrica.Fluid;

namespace Industrica.Recipe.ExtendedRecipe
{
    public record FluidIngredient(TechType TechType, int Amount = 1) : ExtendedIngredient<FluidStack>(TechType, Amount)
    {
        public override bool Test(FluidStack fluidStack)
        {
            return TechType == fluidStack.techType;
        }
    }
}
