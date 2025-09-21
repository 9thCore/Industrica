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
    public static class CrusherRecipes
    {
        public static IEnumerator Register(WaitScreenHandler.WaitScreenTask task)
        {
            task.Status = CrusherLoadingKey.Translate(1, CrusherRecipeSteps);
            yield return null;

            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceTitaniumCopper.TechType, ItemsBasic.CrushedResourceTitaniumCopper.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceCopperSilver.TechType, ItemsBasic.CrushedResourceCopperSilver.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceSilverGold.TechType, ItemsBasic.CrushedResourceSilverGold.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceQuartzDiamond.TechType, ItemsBasic.CrushedResourceQuartzDiamond.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceLeadUraninite.TechType, ItemsBasic.CrushedResourceLeadUraninite.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceMagnetiteLithium.TechType, ItemsBasic.CrushedResourceMagnetiteLithium.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceRubyKyanite.TechType, ItemsBasic.CrushedResourceRubyKyanite.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceLithiumNickel.TechType, ItemsBasic.CrushedResourceLithiumNickel.TechType);
            RegisterBasicMixProcessing(ItemsBasic.OreVeinResourceCrashPowderSulfur.TechType, ItemsBasic.CrushedResourceCrashPowderSulfur.TechType);
        }

        private static void RegisterBasicMixProcessing(TechType input, TechType output)
        {
            RegisterBasic(new ItemIngredient(input, 3), output, count: 4, craftTime: 30f);
        }

        private static void RegisterBasic(ItemIngredient input, TechType output, int count = 1, float craftTime = 5f, List<RecipeUtil.IPrefabModifier> modifiers = null)
        {
            modifiers ??= new();
            modifiers.Add(new RecipeUtil.UnlockRequirement(input.TechType));
            modifiers.Add(new RecipeUtil.UnlockRequirement(BuildableCrusher.Info.TechType));

            CrusherRecipeHandler.Register(
                outputs: new CrusherRecipeHandler.Recipe.Output[]
                {
                    new(output, count)
                },
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

        public const string CrusherLoadingKey = "IndustricaLoading_CrusherRecipes";
        public const int CrusherRecipeSteps = 1;
    }
}
