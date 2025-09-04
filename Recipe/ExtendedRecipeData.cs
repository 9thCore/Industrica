using Nautilus.Crafting;
using System.Collections.Generic;

namespace Industrica.Recipe
{
    public class ExtendedRecipeData : RecipeData
    {
        public float CraftTime { get; set; }

        public ExtendedRecipeData CreateCopy()
        {
            return new ExtendedRecipeData
            {
                Ingredients = new(Ingredients),
                CraftTime = CraftTime,
                craftAmount = craftAmount,
                LinkedItems = new(LinkedItems)
            };
        }
    }
}
