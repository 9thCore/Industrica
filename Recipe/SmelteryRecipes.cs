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
            RegisterBasic(ItemsBasic.OreVeinResourceTitaniumCopper.TechType, TechType.Titanium, craftTime: 30f);
        }
        
        private static void RegisterBasic(TechType input, TechType output, int count = 1, float craftTime = 5f, List<RecipeUtil.IPrefabModifier> modifiers = null)
        {
            modifiers ??= new();
            modifiers.Add(new RecipeUtil.UnlockRequirement(input));
            modifiers.Add(new RecipeUtil.UnlockRequirement(BuildableSmeltery.Info.TechType));

            SmelteryRecipeHandler.Register(
                outputs: new SmelteryRecipeHandler.Recipe.Output[]
                {
                    new(output, count),
                    new(ItemsBasic.Slag.TechType, 1)
                },
                heatLevel: SmelteryRecipeHandler.HeatLevel.Low,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(input, 1)
                    },
                    CraftTime = craftTime
                },
                modifiers: modifiers);
        }
    }
}
