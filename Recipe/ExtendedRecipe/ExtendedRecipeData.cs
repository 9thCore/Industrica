using Nautilus.Crafting;
using System.Collections.Generic;
using System.Linq;

namespace Industrica.Recipe.ExtendedRecipe
{
    public class ExtendedRecipeData
    {
        public List<TechType> LinkedItems = new();
        public List<ItemIngredient> ItemIngredients = new();
        public List<ItemIngredient> ItemCatalysts = new();
        public int CraftAmount { get; set; }
        public float CraftTime { get; set; }

        public void HandleCatalysts()
        {
            if (ItemCatalysts == null
                || ItemCatalysts.Count == 0)
            {
                return;
            }

            foreach (Ingredient catalyst in ItemCatalysts)
            {
                TechType fakeIngredient = ExtraIngredientHelper.GetOrCreateCatalystCloneFor(catalyst.techType);
                ItemIngredients.Add(new ItemIngredient(fakeIngredient, catalyst.amount));
            }

            ItemCatalysts = null;
        }

        public ExtendedRecipeData CreateCopy()
        {
            return new ExtendedRecipeData
            {
                ItemIngredients = new(ItemIngredients),
                ItemCatalysts = new(ItemCatalysts),
                CraftTime = CraftTime,
                CraftAmount = CraftAmount,
                LinkedItems = new(LinkedItems)
            };
        }

        public static implicit operator RecipeData(ExtendedRecipeData extendedRecipeData)
        {
            return new()
            {
                Ingredients = new List<Ingredient>(extendedRecipeData.ItemIngredients.Select(ingredient => (Ingredient)ingredient)),
                craftAmount = extendedRecipeData.CraftAmount,
                LinkedItems = extendedRecipeData.LinkedItems
            };
        }
    }
}
