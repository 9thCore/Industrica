using Industrica.Recipe.Handler;
using Nautilus.Crafting;
using System.Collections.Generic;

namespace Industrica.Recipe
{
    public class ExtendedRecipeData : RecipeData
    {
        public List<Ingredient> Catalysts = new();
        public float CraftTime { get; set; }

        public void HandleCatalysts()
        {
            if (Catalysts == null
                || Catalysts.Count == 0)
            {
                return;
            }

            foreach (Ingredient catalyst in Catalysts)
            {
                TechType fakeIngredient = GeneralFakeIngredients.GetOrCreateCatalystCloneFor(catalyst.techType);
                Ingredients.Add(new Ingredient(fakeIngredient, catalyst.amount));
            }

            Catalysts = null;
        }

        public void HandleCraftTime()
        {
            TechType fakeIngredient = GeneralFakeIngredients.GetOrCreateTimeIngredientFor(CraftTime);
            Ingredients.Add(new Ingredient(fakeIngredient, 1));
        }

        public ExtendedRecipeData CreateCopy()
        {
            return new ExtendedRecipeData
            {
                Ingredients = new(Ingredients),
                Catalysts = new(Catalysts),
                CraftTime = CraftTime,
                craftAmount = craftAmount,
                LinkedItems = new(LinkedItems)
            };
        }
    }
}
