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

            task.Status = SmelteryLoadingKey.Translate(2, SmelteryRecipeSteps);
            yield return null;

            RegisterCrushedProcessing(ItemsBasic.CrushedResourceTitaniumCopper.TechType, TechType.Titanium);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceCopperSilver.TechType, TechType.Copper);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceSilverGold.TechType, TechType.Silver);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceQuartzDiamond.TechType, TechType.Quartz);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceLeadUraninite.TechType, TechType.Lead);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceMagnetiteLithium.TechType, TechType.Magnetite);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceRubyKyanite.TechType, TechType.AluminumOxide);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceLithiumNickel.TechType, TechType.Lithium);
            RegisterCrushedProcessing(ItemsBasic.CrushedResourceCrashPowderSulfur.TechType, TechType.CrashPowder);
        }

        private static void RegisterBasicMixProcessing(TechType input, TechType output)
        {
            RegisterBasic(new ItemIngredient(input, 3), output, craftTime: 30f);
        }

        private static void RegisterCrushedProcessing(TechType input, TechType output)
        {
            RegisterBasic(new ItemIngredient(input, 3), output, craftTime: 25f);
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
        public const int SmelteryRecipeSteps = 2;
    }
}
