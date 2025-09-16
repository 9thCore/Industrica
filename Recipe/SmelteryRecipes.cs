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
            RegisterBasic(new Ingredient(input, 3), output, craftTime: 30f);
        }
        
        private static void RegisterBasic(Ingredient input, TechType output, int count = 1, float craftTime = 5f, List<RecipeUtil.IPrefabModifier> modifiers = null)
        {
            modifiers ??= new();
            modifiers.Add(new RecipeUtil.UnlockRequirement(input.techType));
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
                        input
                    },
                    CraftTime = craftTime
                },
                modifiers: modifiers);
        }
    }
}
