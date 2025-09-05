using Industrica.Buildable.Processing;
using Industrica.Item.Generic;
using Industrica.Recipe.Handler;
using Industrica.Utility;
using System.Collections.Generic;

namespace Industrica.Recipe
{
    public static class SmelteryRecipes
    {
        public static void Register()
        {
            SmelteryRecipeHandler.Register(
                outputs: new SmelteryRecipeHandler.Recipe.Output[]
                {
                    new SmelteryRecipeHandler.Recipe.Output(TechType.Titanium, 1),
                    new SmelteryRecipeHandler.Recipe.Output(ItemsBasic.Slag.TechType, 1)
                },
                heatLevel: SmelteryRecipeHandler.HeatLevel.Low,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceTitaniumCopper.TechType, 1)
                    },
                    CraftTime = 30f
                },
                modifiers: new List<RecipeUtil.IPrefabModifier>()
                {
                    new RecipeUtil.UnlockRequirement(BuildableSmeltery.Info.TechType)
                });
        }
    }
}
