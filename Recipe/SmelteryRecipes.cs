using Industrica.Buildable.Processing;
using Industrica.Item.Generic;
using Industrica.Recipe.ExtendedRecipe;
using Industrica.Recipe.Handler;
using Industrica.Utility;
using Nautilus.Handlers;
using System.Collections;
using System.Collections.Generic;

namespace Industrica.Recipe
{
    public static class SmelteryRecipes
    {
        public static IEnumerator Register(WaitScreenHandler.WaitScreenTask task)
        {
            task.Status = SmelteryLoadingKey.Translate(1, SmelteryRecipeSteps);
            yield return null;

            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceTitaniumCopper.TechType, TechType.Titanium);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceCopperSilver.TechType, TechType.Copper);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceSilverGold.TechType, TechType.Silver);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceQuartzDiamond.TechType, TechType.Quartz);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceLeadUraninite.TechType, TechType.Lead);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceMagnetiteLithium.TechType, TechType.Magnetite);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceRubyKyanite.TechType, TechType.AluminumOxide);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceLithiumNickel.TechType, TechType.Lithium);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceCrashPowderSulfur.TechType, TechType.CrashPowder);
        }

        private static void RegisterBasicMixProcessing(TechType input, TechType output)
        {
            RegisterBasic(new ItemIngredient(input, 3), output, craftTime: 30f);
        }
        
        private static void RegisterBasic(ItemIngredient input, TechType output, int count = 1, float craftTime = 5f, List<RecipeUtil.IPrefabModifier> modifiers = null)
        {
            modifiers ??= new();
            modifiers.Add(new RecipeUtil.UnlockRequirement(input.TechType));
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
                    ItemIngredients =
                    {
                        input
                    },
                    CraftTime = craftTime
                },
                modifiers: modifiers);
        }

        public const string SmelteryLoadingKey = "IndustricaLoading_SmelteryRecipes";
        public const int SmelteryRecipeSteps = 1;
    }
}
