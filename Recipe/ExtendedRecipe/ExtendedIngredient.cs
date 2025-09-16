namespace Industrica.Recipe.ExtendedRecipe
{
    public abstract record ExtendedIngredient<T>(TechType TechType, int Amount = 1)
    {
        public abstract bool Test(T input);

        public static implicit operator Ingredient(ExtendedIngredient<T> extendedIngredient)
        {
            return new Ingredient(extendedIngredient.TechType, extendedIngredient.Amount);
        }
    }
}
