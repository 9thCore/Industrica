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
        public List<FluidIngredient> FluidIngredients = new();

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
                FluidIngredients = new(FluidIngredients),
                CraftTime = CraftTime,
                CraftAmount = CraftAmount,
                LinkedItems = new(LinkedItems)
            };
        }

        public static implicit operator RecipeData(ExtendedRecipeData extendedRecipeData)
        {
            List<Ingredient> pdaIngredients = new();
            pdaIngredients.AddRange(extendedRecipeData.ItemIngredients.Select(ingredient => (Ingredient)ingredient));
            pdaIngredients.AddRange(extendedRecipeData.FluidIngredients.Select(ingredient => (Ingredient)ingredient));

            return new()
            {
                Ingredients = pdaIngredients,
                craftAmount = extendedRecipeData.CraftAmount,
                LinkedItems = extendedRecipeData.LinkedItems
            };
        }
    }
}
